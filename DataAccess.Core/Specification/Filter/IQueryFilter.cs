using System.Linq;

namespace DataAccess.Core.Specification.Filter
{
    public interface IQueryFilter<TEntity> where TEntity : IDbEntity
    {
        IQueryable<TEntity> GetSatisfiedItems(IQueryable<TEntity> src);
    }
}
