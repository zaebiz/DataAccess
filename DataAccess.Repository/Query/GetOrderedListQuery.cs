using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DataAccess.Repository.Repository;
using DataAccess.Repository.Specification;

namespace DataAccess.Repository.Query
{
    /// <summary>
    /// ����� ��� �������� ��������� �� �� �������
    /// Spec - �������� ������� (����������, ���������, ����������)
    /// ����� �� �������������, � �������������� ����� � ����, �������� ���������� ���
    /// </summary>
    public class GetOrderedListQuery<TEntity, TSortKey>
        : GetListQuery<TEntity>
        , IDbQuery<List<TEntity>, OrderedQuerySpec<TEntity, TSortKey>>
        where TEntity : class, IDbEntity
    {
        public GetOrderedListQuery(DbContext db) : base(db)
        {
            // todo ���������������� Spec � ������� �������
            //Spec = new 
        }

        public new OrderedQuerySpec<TEntity, TSortKey> Spec { get; set; }

        protected override IQueryable<TEntity> Execute()
        {
            var queryable = _db.GetOrderedList(Spec);
            if (AttachResultToContext)
                queryable = queryable.AsNoTracking();

            return queryable;
        }
    }
}