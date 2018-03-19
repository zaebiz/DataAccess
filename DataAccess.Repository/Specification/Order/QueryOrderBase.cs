using System;
using System.Linq.Expressions;
using DataAccess.Repository.Repository;

namespace DataAccess.Repository.Specification.Order
{
    public class QueryOrderBase<TEntity, TSortKey>
       : IQueryOrder<TEntity, TSortKey>
       where TEntity : class, IDbEntity
    {
        public int Direction { get; set; }
        public Expression<Func<TEntity, TSortKey>> Expression { get; set; }
    }
}
