using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Core.Specification;
using DataAccess.Core.Specification.Filter;

namespace DataAccess.Core
{
    public interface IRepository
    {
        /// <summary>
        /// получить контекст которым "пользуется" данный репозиторий
        /// </summary>
        TDbContext GetDatabaseContext<TDbContext>() where TDbContext : class;

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
        /// получить список сущностей по "спецификации" - набору правил описывающему параметры запроса:
        /// </summary>
        IQueryable<TEntity> GetList<TEntity>(QuerySpec<TEntity> spec) where TEntity : class, IDbEntity;

        /// <summary>
        /// Добавить новую сущность в контекст
        /// </summary>
        void Add<TEntity>(TEntity entity) where TEntity : class, IDbEntity;

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
