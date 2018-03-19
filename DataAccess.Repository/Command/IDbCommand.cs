using System.Threading.Tasks;

namespace DataAccess.Repository.Command
{
    /// <summary>
    /// ��� "�������" (Command) - ����� �������� ���������� ������
    /// ����� ���������� ��������� ��������
    /// </summary>
    public interface IDbCommand<in TParam, TResult>
    {
        TResult Execute(TParam data);
        Task<TResult> ExecuteAsync(TParam data);
    }
}