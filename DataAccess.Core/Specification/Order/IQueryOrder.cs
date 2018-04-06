using System.Linq;

namespace DataAccess.Core.Specification.Order
{
    public interface IQueryOrder<TEntity> where TEntity : class, IDbEntity
    {
        IQueryable<TEntity> OrderItems(IQueryable<TEntity> src);
    }

}
