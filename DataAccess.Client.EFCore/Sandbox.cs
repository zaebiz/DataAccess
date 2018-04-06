using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Client.EFCore.Models;
using DataAccess.Core;
using DataAccess.Core.Specification;
using DataAccess.Core.Specification.Filter;
using DataAccess.Core.Specification.Join;
using DataAccess.Core.Specification.Paging;
using DataAccess.Repository.DataService;
using DataAccess.Repository.EFCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Client.EFCore
{
    public class Sandbox
    {
        private IRepository _repo;
        private IDataService<Blog> _blogSvc;


        public Sandbox(BlogContext ctx)
        {
            _repo = new RepositoryBase<BlogContext>(ctx);
            _blogSvc= new DataServiceBase<Blog>(_repo);
        }

        public async Task Test()
        {
            var blogs = await _blogSvc.GetItemsList(new QuerySpec<Blog>()
            {
                Filter = new QueryFilterBase<Blog>(x => x.Author.Name.StartsWith("Ni")),
                Paging = new QueryPaging(20, 0),
                Join = new QueryJoinBase<Blog>(
                    src => src
                        .Include(x => x.Author)
                        .Include(x => x.BlogPosts)
                )
            });

            var count = await _blogSvc.GetItemsCount(new QueryFilterBase<Blog>());

            await _blogSvc.AddAndSave(new Blog()
            {
                Id = 20000,
                UserId = 3,
                CreateDate = DateTime.Now,
                Name = "new blog"
            });

            var count2 = await _blogSvc.GetItemsCount(new QueryFilterBase<Blog>());

            return;

        }
    }
}
