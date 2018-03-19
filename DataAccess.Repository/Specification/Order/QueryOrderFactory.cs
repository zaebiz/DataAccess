using System;
using System.Linq.Expressions;
using DataAccess.Repository.Repository;

namespace DataAccess.Repository.Specification.Order
{
    public class QueryOrderFactory<TEntity> where TEntity : class, IDbEntity
    {
        public static IQueryOrder<TEntity, TColumn> Create<TColumn>(Expression<Func<TEntity, TColumn>> expression, int direction = 0)
        {
            return new QueryOrderBase<TEntity, TColumn>()
            {
                Expression = expression,
                Direction = direction
            };
        }

        public static IQueryOrder<TEntity, int> Create()
        {
            return Create(x => x.Id);
        }
    }
}
