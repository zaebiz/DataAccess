using System.Threading.Tasks;

namespace DataAccess.CQRS.Models
{
    /// <summary>
    /// ��� "������" (Query) - �������� ������������ ������ , �� ����� ����� ������ ������ 
    /// </summary>
    public interface IDbQuery<TEntity, TSpecification> //where TEntity : class
    {
        /// <summary>
        /// ����� ��������� (������������) ����������� ���������� � ������������ ������
        /// </summary>
        TSpecification Spec { get; set; }

        TEntity GetResult();
        Task<TEntity> GetResultAsync();
    }

    // todo ����� ��������������� ��� �������, �������� ���������� � ����������� �������, ��������
    //public interface IDbQuery<out TEntity, TSpecification> 
    //    where TEntity : class
    //{
    //    TSpecification Spec { get; set; }
    //    TEntity GetResult();
    //}

    //public interface IDbAsyncQuery<TEntity, TSpecification> 
    //    : IDbQuery<Task<TEntity>, TSpecification>
    //    where TEntity : class
    //{
    //}
}