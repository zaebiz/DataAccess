using System.Data.Entity;
using DataAccess.Repository.Repository;

namespace DataAccess.CQRS.Models
{
    /// <summary>
    /// базовый класс для операций, работающийх с БД
    /// </summary>
    public class DbOperationBase
    {
        protected DbContext _context;
        protected IRepository _repo;

        protected DbOperationBase(DbContext ctx)
        {
            _context = ctx;
            _repo = new RepositoryBase<DbContext>(ctx);
        }
    }
}
