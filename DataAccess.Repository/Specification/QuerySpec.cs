using DataAccess.Repository.Repository;
using DataAccess.Repository.Specification.Filter;
using DataAccess.Repository.Specification.Join;
using DataAccess.Repository.Specification.Order;
using DataAccess.Repository.Specification.Paging;

namespace DataAccess.Repository.Specification
{
    /// <summary>
    /// Набор параметров, кастомизирующих запрос к БД
    /// </summary>
    public class QuerySpec<TEntity> where TEntity : class, IDbEntity
    {
        public QuerySpec()
        {
            Paging = new QueryPaging(20, 0);
            DefaultOrder = QueryOrderFactory<TEntity>.Create();
        }

        /// <summary>
        /// объект, содержащий параметры для фильтрации сущностей
        /// </summary>
        public IQueryFilter<TEntity> Filter { get; set; }

        /// <summary>
        /// объект, содержащий информацию о включении в результаты запроса доп. таблиц
        /// </summary>
        public IQueryJoin<TEntity> Join { get; set; }

        /// <summary>
        /// пагинация
        /// </summary>
        public IQueryPaging Paging { get; set; }


        // пейджинг без сортировки не работает
        public IQueryOrder<TEntity, int> DefaultOrder { get; }

        /// <summary>
        /// убирать ли полученную сущность из контекста после получения
        /// </summary>
        public bool AsNoTracking { get; set; } = true;
    }
}
