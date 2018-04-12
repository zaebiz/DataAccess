using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess.Core.Specification.Filter
{
    // todo при необходимости можно вынести параметры фильтрации в отдельный класс
    public class QueryFilterBase<TEntity>
        : IQueryFilter<TEntity>
        where TEntity : class, IDbEntity
    {
        /// <summary>
        /// Id, по которому осуществляется поиск
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Список Id, по которым осуществляется поиск
        /// </summary>
        public List<int> IdList { get; set; }

        private Expression<Func<TEntity, bool>> _filter { get; set; }

        public QueryFilterBase()
        {
            IdList = new List<int>();
        }

        public QueryFilterBase(Expression<Func<TEntity, bool>> filter)
        {
            _filter = filter;
        }

        public virtual IQueryable<TEntity> GetSatisfiedItems(IQueryable<TEntity> src)
        {
            if (Id.GetValueOrDefault() > 0)
                src = src.Where(x => x.Id == this.Id);

            if (IdList != null && IdList.Count > 0)
            {
                src = src.Where(x => IdList.Contains(x.Id));
                // src = src.Where(x => this.IdList.Any(param => param == x.Id));
            }

            if (_filter != null)
                src = src.Where(_filter);

            return src;
        }
    }
}
