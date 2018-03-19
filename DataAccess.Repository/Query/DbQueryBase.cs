using System.Data.Entity;
using DataAccess.Repository.Command;

namespace DataAccess.Repository.Query
{
    /// <summary>
    /// базовый класс для запросов (Queries) к БД
    /// в данный момент аналог DbCommandBase
    /// </summary>
    public class DbQueryBase : DbCommandBase
    {
        public DbQueryBase(DbContext ctx) : base(ctx)
        {
            ctx.Configuration.AutoDetectChangesEnabled = false;            
        }
    }
    
}
