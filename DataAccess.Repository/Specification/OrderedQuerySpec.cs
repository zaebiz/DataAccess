using DataAccess.Repository.Repository;
using DataAccess.Repository.Specification.Order;

namespace DataAccess.Repository.Specification
{
    /// <summary>
    /// ����� ����������, ��������������� ������ � ��, ��������� ���������� ������
    /// </summary>
    public class OrderedQuerySpec<TEntity, TSortKey> where TEntity : class, IDbEntity
    {
        public OrderedQuerySpec()
        {
            BaseSpec = new QuerySpec<TEntity>();
        }

        public OrderedQuerySpec(QuerySpec<TEntity> baseSpec)
        {
            BaseSpec = baseSpec;
        }

        public OrderedQuerySpec(QuerySpec<TEntity> baseSpec, IQueryOrder<TEntity, TSortKey> order)
        {
            BaseSpec = baseSpec;
            Order = order;
        }

        /// <summary>
        /// ����������� ����� ���������� �������
        /// </summary>
        public QuerySpec<TEntity> BaseSpec;

        /// <summary>
        /// ������ ����������� ��������� ����������
        /// </summary>
        public IQueryOrder<TEntity, TSortKey> Order { get; set; }
    }
}