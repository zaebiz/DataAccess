using System;
using System.Linq.Expressions;
using DataAccess.Repository.Repository;

namespace DataAccess.Repository.Specification.Order
{
    public interface IQueryOrder<TEntity, TSortKey> 
        where TEntity : class, IDbEntity
    {
        int Direction { get; set; }
        Expression<Func<TEntity, TSortKey>> Expression { get; set; }
    }
   
}
