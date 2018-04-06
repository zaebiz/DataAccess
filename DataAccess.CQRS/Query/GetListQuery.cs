using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Core;
using DataAccess.Core.Specification;
using DataAccess.CQRS.Models;

namespace DataAccess.CQRS.Query
{
    /// <summary>
    /// класс дл€ запросов сущностей из Ѕƒ списком
    /// Spec - параметры запроса (фильтраци€, пагинаци€)
    /// можно не наследоватьс€, а инстанцировать пр€мо в коде, указыва€ конкретный тип
    /// </summary>
    public class GetListQuery<TEntity>
        : DbQueryBase
        , IDbQuery<List<TEntity>, QuerySpec<TEntity>> 
        where TEntity : class, IDbEntity
    {

        public GetListQuery(DbContext ctx, QuerySpec<TEntity> spec) : base(ctx)
        {
            Spec = spec ?? new QuerySpec<TEntity>();

            ctx.Configuration.LazyLoadingEnabled = false;
            ctx.Configuration.ProxyCreationEnabled = false;
        }

        protected virtual IQueryable<TEntity> Execute()
            => _repo.GetList(Spec);

        #region IDbQuery members

        public QuerySpec<TEntity> Spec { get; set; }

        public List<TEntity> GetResult()
            => Execute().ToList();

        public async Task<List<TEntity>> GetResultAsync()
            => await Execute().ToListAsync();

        #endregion

    }
}