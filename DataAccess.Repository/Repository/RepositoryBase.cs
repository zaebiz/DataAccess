using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DataAccess.Repository.Specification;
using DataAccess.Repository.Specification.Filter;

namespace DataAccess.Repository.Repository
{
    /// <summary>
    /// Набор методов, реализующих основные операции с БД. 
    /// Особенность: типизирован не сам класс, а каждый метод, для того, 
    /// чтобы 1 и тот же экземпляр репозитория можно было использовать
    /// для запросов к разным наборам сущностей(DbSet-ам) из контекста.
    /// </summary>
    public class RepositoryBase<TContext> : IRepository where TContext : DbContext
    {
        private readonly TContext _db;

        public RepositoryBase(TContext db)
        {
            _db = db;
            _db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public TDbContext GetDatabaseContext<TDbContext>() where TDbContext : DbContext
            => _db as TDbContext;

        public IDbSet<TEntity> GetDbSet<TEntity>() where TEntity : class, IDbEntity
            => _db.Set<TEntity>();

        public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IDbEntity
            => GetDbSet<TEntity>();

        public IQueryable<TEntity> GetFilteredQueryable<TEntity>(IQueryFilter<TEntity> spec) where TEntity : class, IDbEntity
            => GetQueryable<TEntity>()
                .ApplyFilter(spec);

        public async Task<TEntity> GetItemById<TEntity>(GetByIdSpec<TEntity> spec) where TEntity : class, IDbEntity
            => await
                GetQueryable<TEntity>()
                    .ApplyJoin(spec.Join)
                    .ApplyTracking(spec.AsNoTracking)
                    .FirstOrDefaultAsync(x => x.Id == spec.Id);

        public IQueryable<TEntity> GetList<TEntity>(QuerySpec<TEntity> spec) where TEntity : class, IDbEntity
        {
            var queryable = GetFilteredQueryable(spec.Filter)
                .ApplyJoin(spec.Join)
                .ApplyTracking(spec.AsNoTracking);

            if (spec.Paging != null) //сортировка не будет работать если нет пагинации
                queryable = queryable.ApplyOrder(spec.DefaultOrder);

            return queryable.ApplyPaging(spec.Paging);
        }

        public IQueryable<TEntity> GetOrderedList<TEntity, TSortKey>(OrderedQuerySpec<TEntity, TSortKey> spec)
            where TEntity : class, IDbEntity
        {
            if (spec.Order == null)
                throw new Exception("Не указана сортировка. Используйте QuerySpecification<>");

            return GetFilteredQueryable(spec.BaseSpec.Filter)
                .ApplyJoin(spec.BaseSpec.Join)
                .ApplyTracking(spec.BaseSpec.AsNoTracking)
                .ApplyOrder(spec.Order)
                .ApplyPaging(spec.BaseSpec.Paging);
        }

        public void AddOrUpdate<TEntity>(TEntity entity) where TEntity : class, IDbEntity
        {
            if (entity.Id == default(int))
            {
                _db.Set<TEntity>().Add(entity);
            }
            else
            {
                var exist = _db.Set<TEntity>().Find(entity.Id);
                if (exist == null)
                    _db.Set<TEntity>().Add(entity);
                else
                    _db.Entry(exist).CurrentValues.SetValues(entity);
            }

            //bool entityExist = entity.Id > 0;

            //_db.Entry(entity).State = entityExist
            //    ? EntityState.Modified
            //    : EntityState.Added;
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class, IDbEntity
        {
            _db.Set<TEntity>().Remove(entity);
        }

        public void Remove<TEntity>(int entityId) where TEntity : class, IDbEntity
        {
            var entity = _db.Set<TEntity>().Find(entityId);
            if (entity == null)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Requested entity not found");

            Remove(entity);
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IDbEntity
        {
            _db.Set<TEntity>().RemoveRange(entities);
        }

        public async Task RemoveRange<TEntity>(IEnumerable<int> entityIdList) where TEntity : class, IDbEntity
        {
            var entityList = await _db.Set<TEntity>()
                .Where(x => entityIdList.Contains(x.Id))
                .ToListAsync();

            if (entityList.Any())
            {
                RemoveRange(entityList);
            }
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        //todo: mvd написать перегрузку этого метода для сущностей у который указатель на родителя int?
        public async Task<List<TChild>> TrackChildChanges<TParent, TChild>(
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
            var oldItems = await _db.Set<TChild>().Where(lam).ToListAsync();

            //сравниваем с новыми
            var itemsForDelete = oldItems.Where(o => newItems.All(n => n.Id != o.Id)).ToList();
            var itemsForAdd = newItems.Where(o => o.Id == default(int)).ToList();
            var itemsForUpdate = newItems.Where(o => o.Id != default(int)).ToList();

            //удаляем старые
            foreach (var entity in itemsForDelete)
            {
                _db.Entry(entity).State = EntityState.Deleted;
            }

            //добавляем новые
            foreach (var entity in itemsForAdd)
            {
                _db.Entry(entity).State = EntityState.Added;
            }

            foreach (var entity in itemsForUpdate)
            {
                //проверяем контекст и базу
                //(все элементы коллекции для обновления должны быть в контексте, тк мы раньше уже доставали все значение из базы по ключу родителя)
                //тем не менее в случае рассинхронизации с клиента может прийти элементы которых уже нет в дб, для этого мы проверяем и контекст и базу
                var exist = await _db.Set<TChild>().FindAsync(entity.Id);
                if (exist != null)
                {
                    //отсоединяем существующую сущность из контекста
                    _db.Entry(exist).State = EntityState.Detached;
                    //присоединяем новую
                    _db.Entry(entity).State = EntityState.Modified;
                }
                //если с клиента пришел элемент которого нет в базе, ничего не делаем
                //else
                //{
                //    _db.Set<TChild>().Add(entity);
                //}
            }

            return newItems;
        }
    }
}