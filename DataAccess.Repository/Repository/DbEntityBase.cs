using System.ComponentModel.DataAnnotations;

namespace DataAccess.Repository.Repository
{
    public class DbEntityBase : IDbEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
