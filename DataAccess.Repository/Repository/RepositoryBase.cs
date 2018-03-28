using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DataAccess.Repository.Extensions;
using DataAccess.Repository.Specification;
using DataAccess.Repository.Specification.Filter;
using DataAccess.Repository.Specification.Order;

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
            //сортировка не будет работать если нет пагинации
            if (spec.Paging != null)
            {
                spec.Order = spec.Order ?? new QueryOrderBase<TEntity>(x => x.OrderBy(i => i.Id));
            }

            return GetFilteredQueryable(spec.Filter)
                .ApplyJoin(spec.Join)
                .ApplyTracking(spec.AsNoTracking)
                .ApplyOrder(spec.Order)
                .ApplyPaging(spec.Paging);
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

    }
}