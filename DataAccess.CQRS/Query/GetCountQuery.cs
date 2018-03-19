using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.CQRS.Models;
using DataAccess.Repository.Repository;
using DataAccess.Repository.Specification.Filter;

namespace DataAccess.CQRS.Query
{
    public class GetCountQuery<TEntity>
        : DbQueryBase
        , IDbQuery<int, IQueryFilter<TEntity>>
        where TEntity : class, IDbEntity
    {
        public GetCountQuery(DbContext ctx) : base(ctx)
        {
        }

        public IQueryFilter<TEntity> Spec { get; set; }

        public int GetResult()
        {
            return _repo
                .GetFilteredQueryable(Spec)
                .Count();
        }

        public async Task<int> GetResultAsync()
        {
            return await _repo
                .GetFilteredQueryable(Spec)
                .CountAsync();
        }

        
    }
}
