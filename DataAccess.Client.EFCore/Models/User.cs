using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Core;

namespace DataAccess.Client.EFCore.Models
{
    [Table("Users")]
    public class User : IDbEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime RegistrationDate { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
    }
}
