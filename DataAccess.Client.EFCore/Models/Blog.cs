using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Core;

namespace DataAccess.Client.EFCore.Models
{
    [Table("Blogs")]
    public class Blog : IDbEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public DateTime CreateDate { get; set; }

        [ForeignKey("Author")]
        public int UserId { get; set; }
        public virtual User Author { get; set; }

        public virtual ICollection<Post> BlogPosts { get; set; }
    }
}