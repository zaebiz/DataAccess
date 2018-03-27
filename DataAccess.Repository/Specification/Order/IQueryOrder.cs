using System;
using System.Linq;
using System.Linq.Expressions;
using DataAccess.Repository.Repository;

namespace DataAccess.Repository.Specification.Order
{
    public interface IQueryOrder<TEntity> where TEntity : class, IDbEntity
    {
        IQueryable<TEntity> OrderItems(IQueryable<TEntity> src);
    }

}
