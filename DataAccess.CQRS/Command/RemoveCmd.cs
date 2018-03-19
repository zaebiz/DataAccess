using System.Data.Entity;
using System.Threading.Tasks;
using DataAccess.CQRS.Models;
using DataAccess.Repository.Repository;

namespace DataAccess.CQRS.Command
{
    /// <summary>
    /// Базовый класс для команд удаления некой сущности
    /// можно не наследоваться, а инстанцировать прямо в коде, указывая конкретный тип
    /// </summary>
    public class RemoveCmd<TEntity>
        : DbOperationBase
        , IDbCommand<TEntity, bool>
        where TEntity : class, IDbEntity
    {
        public RemoveCmd(DbContext ctx) : base(ctx)
        { }

        public virtual bool Execute(TEntity entity)
        {
            _repo.Remove(entity);
            _repo.SaveChanges();
            return true;
        }

        public virtual async Task<bool> ExecuteAsync(TEntity entity)
        {
            _repo.Remove(entity);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}