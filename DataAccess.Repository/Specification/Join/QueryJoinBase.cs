using System;
using System.Linq;
using DataAccess.Repository.Repository;

namespace DataAccess.Repository.Specification.Join
{
    public class QueryJoinBase<TEntity> : IQueryJoin<TEntity> where TEntity : class, IDbEntity
    {
        private Func<IQueryable<TEntity>, IQueryable<TEntity>> _include;

        public QueryJoinBase(Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
        {
            _include = include;
        }

        public IQueryable<TEntity> Include(IQueryable<TEntity> src)
        {
            return _include(src);
        }
    }
}
