using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DataAccess.Repository.Specification;
using DataAccess.Repository.Specification.Filter;

namespace DataAccess.Repository.Repository
{
    public interface IRepository
    {
        /// <summary>
        /// получить контекст которым "пользуется" данный репозиторий
        /// </summary>
        TDbContext GetDatabaseContext<TDbContext>() where TDbContext : DbContext;

        /// <summary>
        /// получить DbSet из контекста, в котором содержатся объекты TEntity
        /// </summary>
        IDbSet<TEntity> GetDbSet<TEntity>() where TEntity : class, IDbEntity;

        /// <summary>
        /// получить DbSet из контекста, в котором содержатся объекты TEntity. Возвращается как объект типа IQueryable
        /// </summary>
        IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IDbEntity;

        /// <summary>
        /// получить список сущностей, отфильтрованых по параметрам, переданным в объекте типа IQueryFilter
        /// </summary>
        IQueryable<TEntity> GetFilteredQueryable<TEntity>(IQueryFilter<TEntity> spec) where TEntity : class, IDbEntity;

        /// <summary>
        /// получить сущность по Id
        /// </summary>
        Task<TEntity> GetItemById<TEntity>(GetByIdSpec<TEntity> spec) where TEntity : class, IDbEntity;

        /// <summary>
        /// получить список сущностей по "спецификации" - набору правил описывающему:
        /// Join (какие таблицы присоединять к результату запроса)
        /// Filter (набор Where-предикатов, фильтрующих сущности)
        /// Paging (параметры пагинации запроса)
        /// </summary>
        IQueryable<TEntity> GetList<TEntity>(QuerySpec<TEntity> spec) where TEntity : class, IDbEntity;

        /// <summary>
        /// получить отсортированный список сущностей по "спецификации" - набору правил описывающему:
        /// Join (какие таблицы присоединять к результату запроса)
        /// Filter (набор Where-предикатов, фильтрующих сущности)
        /// Paging (параметры пагинации запроса)
        /// Order (правило сортировки результата)
        /// </summary>
        IQueryable<TEntity> GetOrderedList<TEntity, TSortKey>(OrderedQuerySpec<TEntity, TSortKey> spec) where TEntity : class, IDbEntity;

        /// <summary>
        /// Создать новую сущность, либо обновить существующую (в контексте)
        /// Операция выбирается в зависимости от поля Id (insert = Id==0)
        /// </summary>
        void AddOrUpdate<TEntity>(TEntity entity) where TEntity : class, IDbEntity;

        /// <summary>
        /// удаление сущности (в контексте)
        /// </summary>
        void Remove<TEntity>(TEntity entity) where TEntity : class, IDbEntity;

        /// <summary>
        /// удаление сущности по Id (в контексте)
        /// </summary>
        void Remove<TEntity>(int entityId) where TEntity : class, IDbEntity;

        /// <summary>
        /// удалить список сущностей
        /// </summary>
        void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IDbEntity;

        /// <summary>
        /// удалить список сущностей по списку Id
        /// </summary>
        Task RemoveRange<TEntity>(IEnumerable<int> entityIdList) where TEntity : class, IDbEntity;

        /// <summary>
        /// сохранить изменения контекста в БД
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// сохранить изменения контекста в БД
        /// </summary>
        Task SaveChangesAsync();        
    }
}
