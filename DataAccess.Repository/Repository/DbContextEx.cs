using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace DataAccess.Repository.Repository
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
                        Type entityType = ObjectContext.GetObjectType(entry.Entity.GetType());

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
    }
}
