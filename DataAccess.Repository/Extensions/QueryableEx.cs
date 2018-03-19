using System.Data.Entity;
using System.Linq;
using DataAccess.Repository.Repository;
using DataAccess.Repository.Specification.Filter;
using DataAccess.Repository.Specification.Join;
using DataAccess.Repository.Specification.Order;
using DataAccess.Repository.Specification.Paging;

namespace DataAccess.Repository.Extensions
{
    public static class QueryableEx
    {
        /// <summary>
        /// отфильтировать IQueryable используя IQueryFilter
        /// </summary>
        public static IQueryable<TEntity> ApplyFilter<TEntity>(this IQueryable<TEntity> src, IQueryFilter<TEntity> filter) where TEntity : class, IDbEntity
        {
            if (filter != null)
                src = filter.GetSatisfiedItems(src);

            return src;
        }

        /// <summary>
        /// включить в результаты запроса доп. таблицы, описанные в IQueryJoin
        /// </summary>
        public static IQueryable<TEntity> ApplyJoin<TEntity>(this IQueryable<TEntity> src, IQueryJoin<TEntity> join) where TEntity : class, IDbEntity
        {
            if (join != null)
                src = join.Include(src);

            return src;
        }

        /// <summary>
        /// пагинация IQueryable на основе IQueryPaging
        /// </summary>
        public static IQueryable<TEntity> ApplyPaging<TEntity>(this IQueryable<TEntity> src, IQueryPaging paging) where TEntity : class, IDbEntity
        {
            if (paging != null)
                src = src.Skip(paging.Offset).Take(paging.PageSize);

            return src;
        }

        /// <summary>
        /// сортировка IQueryable на основе выражения, переданного в IQueryOrder
        /// </summary>
        public static IQueryable<TEntity> ApplyOrder<TEntity, TKey>(this IQueryable<TEntity> src, IQueryOrder<TEntity, TKey> order) where TEntity : class, IDbEntity
        {
            if (order != null)
            {
                if (order.Direction == 0)
                    src = src.OrderBy(order.Expression);
                else
                    src = src.OrderByDescending(order.Expression);
            }

            return src;
        }

        /// <summary>
        /// применения к запросу метода AsNoTracking()
        /// </summary>
        public static IQueryable<TEntity> ApplyTracking<TEntity>(this IQueryable<TEntity> src, bool asNoTracking) where TEntity : class, IDbEntity
        {
            if (asNoTracking)
                src = src.AsNoTracking();

            return src;
        }
    }
}
