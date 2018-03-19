using System.Linq;
using DataAccess.Repository.Repository;

namespace DataAccess.Repository.Specification.Filter
{
    public interface IQueryFilter<TEntity> where TEntity : IDbEntity
    {
        IQueryable<TEntity> GetSatisfiedItems(IQueryable<TEntity> src);
    }
}
