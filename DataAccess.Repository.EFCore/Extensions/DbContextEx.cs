using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DataAccess.Core;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.EFCore.Extensions
{
    public static class DbContextEx
    {
        /// <summary>
        /// Корректно добавитьв контекст граф объектов, ранее отсоединенный от контекста.
        /// (Могут быть проблемы, если у сущности несколько виртуальных свойств одного типа)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rootEntity">Добавляемый объект верхнего уровня</param>
        /// <param name="childTypes">Изменения в св-вах, принадлежащие к данному набору типов будут обработаны контекстом </param>
        public static void AttachObjectGraph<TEntity>(this DbContext context, TEntity rootEntity, HashSet<Type> childTypes) where TEntity : class, IDbEntity
        {
            // вставвесь граф объектов в контекст. т.к. вызываем Add(), все сущности будут помечены как Added
            context.Set<TEntity>().Add(rootEntity);
            // поменяем состояние сущности верхнего уровня, если она не добавлялась
            if (rootEntity.Id != 0)
            {
                context.Entry(rootEntity).State = EntityState.Modified;
            }

            // проходим по всем связанным сущностям, которые также были добавлены в контекст
            foreach (var entry in context.ChangeTracker.Entries<IDbEntity>())
            {
                // пропускаем сущность верхнего уровня . обрабатываем только объекты у которых состояние Added (это те которые мы только что добавили в контекст)
                if (entry.State == EntityState.Added && entry.Entity != rootEntity)
                {
                    // если в childTypes не указано, какие типы дочерних сущностей обрабатывать, то ставим им всем Unchanged, чтобы EF не пытался добавлять их в БД
                    if (childTypes == null || childTypes.Count == 0)
                    {
                        entry.State = EntityState.Unchanged;
                    }
                    else
                    {
                        // получаем оригинальный тип сущности, т.к. бывает что используются dynamic proxy
                        Type entityType = /*ObjectContext.GetObjectType(*/entry.Entity.GetType()/*)*/; //todo mvd: в ef core нет dynamic proxy по инфе с so

                        // если текущий тип обрабатывать не надо, помечаем сущность как Unchanged
                        if (!childTypes.Contains(entityType))
                        {
                            entry.State = EntityState.Unchanged;
                        }
                        else if (entry.Entity.Id != 0)
                        {
                            // если у сущности был Id, метим ее как обновленную, иначе оставляем как добавленную
                            entry.State = EntityState.Modified;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// "очистить" объект контекста - убрать из него все "кешированные" сущности
        /// </summary>
        /// <param name="context"></param>
        public static void DetachAllEntities(this DbContext context)
        {
            var entitiesToDetach = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted || e.State == EntityState.Unchanged);

            foreach (var entity in entitiesToDetach)
            {
                entity.State = EntityState.Detached;
            }
        }

        //todo: mvd написать перегрузку этого метода для сущностей у который указатель на родителя int?
        public static async Task<List<TChild>> TrackChildChanges<TParent, TChild>(
            this DbContext context,
            TParent parent,
            Func<TParent, ICollection<TChild>> refFromParrentToChildCollection,
            Expression<Func<TChild, int>> refFromChildToParrentKey)
            where TParent : class, IDbEntity
            where TChild : class, IDbEntity
        {
            //берем ключ родителя
            var parentId = parent.Id;

            //достаем функцию из выражения
            var funcRefFromChildToParrentKey = refFromChildToParrentKey.Compile();

            //делаем копию коллекции, выбирая только те сущности, у которых не проставлен ключ либо проставлен в 0
            var newItems = refFromParrentToChildCollection(parent).Where(o => funcRefFromChildToParrentKey(o) == default(int) || funcRefFromChildToParrentKey(o) == parentId).ToList();

            //для сущностей у которых не выставлен ключ на родителя, выставляем значения ключа
            newItems.Where(o => funcRefFromChildToParrentKey(o) == default(int)).ToList().ForEach(newItem =>
            {
                ((refFromChildToParrentKey.Body as MemberExpression)?.Member as PropertyInfo)?.SetValue(newItem, parentId, null);
            });

            //создаем лямбду для EF по поиску старых значений
            var keyConst = Expression.Constant(parentId, typeof(int));
            var keyProp = Expression.Convert(refFromChildToParrentKey.Body, typeof(int));
            var lam = Expression.Lambda<Func<TChild, bool>>(Expression.Equal(keyProp, keyConst), refFromChildToParrentKey.Parameters);

            //доставем из базы старые значения
            var oldItems = await context.Set<TChild>().Where(lam).ToListAsync();

            //сравниваем с новыми
            var itemsForDelete = oldItems.Where(o => newItems.All(n => n.Id != o.Id)).ToList();
            var itemsForAdd = newItems.Where(o => o.Id == default(int)).ToList();
            var itemsForUpdate = newItems.Where(o => o.Id != default(int)).ToList();

            //удаляем старые
            foreach (var entity in itemsForDelete)
            {
                context.Entry(entity).State = EntityState.Deleted;
            }

            //добавляем новые
            foreach (var entity in itemsForAdd)
            {
                context.Entry(entity).State = EntityState.Added;
            }

            foreach (var entity in itemsForUpdate)
            {
                //проверяем контекст и базу
                //(все элементы коллекции для обновления должны быть в контексте, тк мы раньше уже доставали все значение из базы по ключу родителя)
                //тем не менее в случае рассинхронизации с клиента может прийти элементы которых уже нет в дб, для этого мы проверяем и контекст и базу
                var exist = await context.Set<TChild>().FindAsync(entity.Id);
                if (exist != null)
                {
                    //отсоединяем существующую сущность из контекста
                    context.Entry(exist).State = EntityState.Detached;
                    //присоединяем новую
                    context.Entry(entity).State = EntityState.Modified;
                }
                //если с клиента пришел элемент которого нет в базе, ничего не делаем
                //else
                //{
                //    _db.Set<TChild>().Add(entity);
                //}
            }

            return newItems;
        }

        public static void AddEntityToContextAndMarkSomePropertiesAsModified<T>(this DbContext context, T entity, params Expression<Func<T, object>>[] updatedProperties) where T : class
        {
            if (!updatedProperties.Any())
                return;

            //attach to context, can throw exception if we already have entity with same primary key
            context.Set<T>().Attach(entity);

            var dbEntityEntry = context.Entry(entity);
            //update explicitly mentioned properties
            foreach (var property in updatedProperties)
            {
                dbEntityEntry.Property(property).IsModified = true;
            }
        }
    }
}
