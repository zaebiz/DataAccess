using System;
using System.Linq;
using System.Linq.Expressions;
using DataAccess.Repository.Repository;

namespace DataAccess.Repository.Specification.Order
{
    /// <summary>
    /// Generic ordering rules incapsulation for QuerySpec objects
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class QueryOrderBase<TEntity> : IQueryOrder<TEntity>
        where TEntity : class, IDbEntity
    {
        private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> _orderFunc;

        public QueryOrderBase(Func<IQueryable<TEntity>, IQueryable<TEntity>> orderFunc)
        {
            _orderFunc = orderFunc;
        }

        public IQueryable<TEntity> OrderItems(IQueryable<TEntity> src)
        {
            return _orderFunc(src);
        }

        /// <summary>
        /// Default order Id => ascending
        /// </summary>
        public static QueryOrderBase<TEntity> DefaultAsc
            => new QueryOrderBase<TEntity>(x => x.OrderBy(i => i.Id));

        /// <summary>
        /// Default order Id => descending
        /// </summary>
        public static QueryOrderBase<TEntity> DefaultDesc
            => new QueryOrderBase<TEntity>(x => x.OrderByDescending(i => i.Id));
    }
}
