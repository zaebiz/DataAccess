using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Core;

namespace DataAccess.Client.EFCore.Models
{
    [Table("Posts")]
    public class Post : IDbEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public DateTime PostDate { get; set; }

        [ForeignKey("Blog")]
        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }
    }
}