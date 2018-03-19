using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DataAccess.CQRS.Models;
using DataAccess.Repository.Repository;
using DataAccess.Repository.Specification;
using DataAccess.Repository.Specification.Order;

namespace DataAccess.CQRS.Query
{
    /// <summary>
    /// класс для запросов сущностей из БД списком
    /// Spec - паремтры запроса (фильтрация, пагинация, сортировка)
    /// можно не наследоваться, а инстанцировать прямо в коде, указывая конкретный тип
    /// </summary>
    public class GetOrderedListQuery<TEntity, TSortKey>
        : GetListQuery<TEntity>
        , IDbQuery<List<TEntity>, OrderedQuerySpec<TEntity, TSortKey>>
        where TEntity : class, IDbEntity
    {
        public GetOrderedListQuery(DbContext db, OrderedQuerySpec<TEntity, TSortKey> spec) : base(db, spec.BaseSpec)
        {
            Spec = spec;
        }

        public new OrderedQuerySpec<TEntity, TSortKey> Spec { get; set; }

        protected override IQueryable<TEntity> Execute()
        {
            return _repo.GetOrderedList(Spec);
        }
    }
}