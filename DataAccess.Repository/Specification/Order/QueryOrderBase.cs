using System;
using System.Linq;
using System.Linq.Expressions;
using DataAccess.Repository.Repository;

namespace DataAccess.Repository.Specification.Order
{
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
    }
}
