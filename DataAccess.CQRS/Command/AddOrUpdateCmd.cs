using System.Data.Entity;
using System.Threading.Tasks;
using DataAccess.CQRS.Models;
using DataAccess.Repository.Repository;

namespace DataAccess.CQRS.Command
{
    /// <summary>
    /// ������� ����� ��� ������ ����������/���������� ����� ��������
    /// ����� �� �������������, � �������������� ����� � ����, �������� ���������� ���
    /// </summary>
    public class AddOrUpdateCmd<TEntity>
        : DbOperationBase
        , IDbCommand<TEntity, TEntity>
        where TEntity : class, IDbEntity
    {
        public AddOrUpdateCmd(DbContext ctx) : base(ctx)
        {}

        public virtual TEntity Execute(TEntity entity)
        {
            _repo.AddOrUpdate(entity);
            _repo.SaveChanges();
            return entity;
        }

        public virtual async Task<TEntity> ExecuteAsync(TEntity entity)
        {
            _repo.AddOrUpdate(entity);
            await _repo.SaveChangesAsync();
            return entity;
        }
    }
}