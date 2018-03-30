using System;
using System.Data.Entity;
using System.Threading.Tasks;
using DataAccess.Core;
using DataAccess.Core.Specification;
using DataAccess.CQRS.Models;

namespace DataAccess.CQRS.Query
{
    /// <summary>
    /// ����� ��� �������� ����� ��������� �� �� �� Id
    /// �� ���� ����� (� ���� �����) �� �������������, � �������������� ����� � ����, �������� ���������� ���
    /// </summary>
    public class GetByIdQuery<TEntity>
        : DbQueryBase
        , IDbQuery<TEntity, GetByIdSpec<TEntity>>
        where TEntity : class, IDbEntity
    {
        public GetByIdQuery(DbContext ctx) : base(ctx)
        {}

        public GetByIdSpec<TEntity> Spec { get; set; }

        //public TEntity GetResult()
        //    => _db.GetItemById<TEntity>(Spec);

        //public async Task<TEntity> GetResultAsync()
        //    => await Task.FromResult(GetResult());

        public TEntity GetResult()
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> GetResultAsync()
            => await _repo.GetItemById(Spec);
    }
}