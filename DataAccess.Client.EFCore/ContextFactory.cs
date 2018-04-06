using System;
using System.Collections.Generic;
using System.Text;
using DataAccess.Client.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Client.EFCore
{
    public class ContextFactory
    {
        private readonly ILoggerFactory _logger;
        private List<User> _users;
        private List<Blog> _blogs;
        private List<Post> _posts;
        private List<string> _tags;
        private Random _r;
        private BlogContext _ctx;

        public ContextFactory(ILoggerFactory logger)
        {
            _logger = logger;
            _users = new List<User>();
            _blogs = new List<Blog>();
            _posts = new List<Post>();

            _tags = new List<string>()
            {
                "health",
                "sport",
                "love",
                "war",
                "toys"
            };

            _r = new Random();
        }

        public BlogContext GetContext()
        {
            var ctx = new BlogContext(
                new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase("Blogs")
                .UseLoggerFactory(_logger)
                .Options);

            return InitDatabase(ctx);
        }

        private BlogContext InitDatabase(BlogContext ctx)
        {
            InitUsers();
            ctx.Users.AddRange(_users);
            ctx.SaveChanges();

            InitBlogs();
            ctx.Blogs.AddRange(_blogs);
            ctx.SaveChanges();

            InitPosts();
            ctx.Posts.AddRange(_posts);
            ctx.SaveChanges();

            return ctx;
        }

        private void InitUsers()
        {
            _users.Add(new User() { Name = "Nikita", RegistrationDate = DateTime.Today });
            _users.Add(new User() { Name = "Ivan", RegistrationDate = DateTime.Today });
            _users.Add(new User() { Name = "Maxim", RegistrationDate = DateTime.Today });
            _users.Add(new User() { Name = "Sergey", RegistrationDate = DateTime.Today });
            _users.Add(new User() { Name = "Nikolay", RegistrationDate = DateTime.Today });
        }

        private void InitBlogs()
        {
            for (int i = 0; i < 100; i++)
            {
                var author = _users[_r.Next(_users.Count)];
                _blogs.Add(new Blog()
                {
                    Name = GetRandomText(10),
                    Author = author,
                    CreateDate = DateTime.Today
                });
            }
        }

        private void InitPosts()
        {
            for (int i = 0; i < 2000; i++)
            {
                var blog = _blogs[_r.Next(_blogs.Count)];
                var tag = _tags[_r.Next(_tags.Count)];

                _posts.Add(new Post()
                {
                    Name = GetRandomText(20),
                    BlogId = blog.Id,
                    Tag = tag,
                    PostDate = DateTime.Now.AddDays(_r.Next(1000) * -1)
                });
            }
        }

        private string GetRandomText(int length)
        {
            var dict = "abcdefghijklmnopqrstuvwxyz1234567890";
            StringBuilder text = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                var symbol = dict[_r.Next(dict.Length)];
                text.Append(symbol);
            }

            return text.ToString();
        }
    }
}
