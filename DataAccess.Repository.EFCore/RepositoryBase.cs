using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Core;
using DataAccess.Core.Specification;
using DataAccess.Core.Specification.Filter;
using DataAccess.Core.Specification.Order;
using DataAccess.Repository.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.EFCore
{
    /// <summary>
    /// Набор методов, реализующих основные операции с БД. 
    /// Особенность: типизирован не сам класс, а каждый метод, для того, 
    /// чтобы 1 и тот же экземпляр репозитория можно было использовать
    /// для запросов к разным наборам сущностей(DbSet-ам) из контекста.
    /// </summary>
    public class RepositoryBase<TContext> : IRepository where TContext : class
    {
        private readonly DbContext _db;

        public RepositoryBase(TContext db)
        {
            _db = db as DbContext;
            if (_db == null)
                throw new ArgumentException();

            // todo LogToOutput ?
            //_db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public TDbContext GetDatabaseContext<TDbContext>() where TDbContext : class 
            => _db as TDbContext;

        public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IDbEntity
            => _db.Set<TEntity>();

        public IQueryable<TEntity> GetFilteredQueryable<TEntity>(IQueryFilter<TEntity> spec) where TEntity : class, IDbEntity
            => GetQueryable<TEntity>()
                .ApplyFilter(spec);

        public async Task<TEntity> GetItemById<TEntity>(GetByIdSpec<TEntity> spec) where TEntity : class, IDbEntity
            => await
                _db.Set<TEntity>()
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

            return _db.Set<TEntity>()
                .ApplyFilter(spec.Filter)
                .ApplyJoin(spec.Join)
                .ApplyTracking(spec.AsNoTracking)
                .ApplyOrder(spec.Order)
                .ApplyPaging(spec.Paging);
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class, IDbEntity
        {
            _db.Set<TEntity>().Add(entity);
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