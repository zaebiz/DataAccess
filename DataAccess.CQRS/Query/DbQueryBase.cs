using System.Data.Entity;
using DataAccess.CQRS.Command;
using DataAccess.CQRS.Models;

namespace DataAccess.CQRS.Query
{
    /// <summary>
    /// базовый класс для запросов (Queries) к БД
    /// </summary>
    public class DbQueryBase : DbOperationBase
    {
        public DbQueryBase(DbContext ctx) : base(ctx)
        {
            ctx.Configuration.AutoDetectChangesEnabled = false;            
        }
    }
    
}
