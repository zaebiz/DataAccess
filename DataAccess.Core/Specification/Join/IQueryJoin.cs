using System.Linq;

namespace DataAccess.Core.Specification.Join
{
    public interface IQueryJoin<T> where T : IDbEntity
    {
        IQueryable<T> Include(IQueryable<T> src);
    }
}
