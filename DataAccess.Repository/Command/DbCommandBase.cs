using System.Data.Entity;
using DataAccess.Repository.Repository;

namespace DataAccess.Repository.Command
{
    /// <summary>
    /// базовый класс для команд, работающийх с БД
    /// содержит контекст и репозиторий
    /// </summary>
    public class DbCommandBase
    {
        protected DbContext _context;
        protected IRepository _db;

        protected DbCommandBase(DbContext ctx)
        {
            _context = ctx;
            _db = new RepositoryBase<DbContext>(ctx);
        }
    }
}
