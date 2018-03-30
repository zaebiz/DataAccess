using System.Linq;
using DataAccess.Core;
using DataAccess.Core.Specification.Filter;
using DataAccess.Core.Specification.Join;
using DataAccess.Core.Specification.Order;
using DataAccess.Core.Specification.Paging;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.EFCore.Extensions
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
        /// сортировка IQueryable на основе IQueryOrder
        /// </summary>
        public static IQueryable<TEntity> ApplyOrder<TEntity>(this IQueryable<TEntity> src, IQueryOrder<TEntity> order) where TEntity : class, IDbEntity
        {
            if (order != null)
            {
                src = order.OrderItems(src);
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
