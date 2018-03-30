using Microsoft.EntityFrameworkCore;

namespace DataAccess.Client.EFCore.Models
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
            //Database.SetInitializer(new BlogInitializer());
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
    }
}
