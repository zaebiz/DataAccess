using System.Linq;
using DataAccess.Repository.Repository;

namespace DataAccess.Repository.Specification.Join
{
    public interface IQueryJoin<T> where T : IDbEntity
    {
        IQueryable<T> Include(IQueryable<T> src);
    }
}
