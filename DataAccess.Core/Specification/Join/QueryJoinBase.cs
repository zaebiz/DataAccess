using System;
using System.Linq;

namespace DataAccess.Core.Specification.Join
{
    public class QueryJoinBase<TEntity> : IQueryJoin<TEntity> where TEntity : class, IDbEntity
    {
        private Func<IQueryable<TEntity>, IQueryable<TEntity>> _includeFunc;

        public QueryJoinBase(Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc)
        {
            _includeFunc = includeFunc;
        }

        public IQueryable<TEntity> Include(IQueryable<TEntity> src)
        {
            return _includeFunc(src);
        }
    }
}
