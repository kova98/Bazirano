using Bazirano.Models;
using Bazirano.Models.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Components
{
    public class PostResponsesViewComponent : ViewComponent
    {
        private IBoardThreadsRepository repository;

        public PostResponsesViewComponent(IBoardThreadsRepository repo)
        {
            repository = repo;
        }

        public IViewComponentResult Invoke(BoardPost post)
        {
            BoardThread thread = repository.BoardThreads.FirstOrDefault(t => t.Posts.Contains(post));

            var responses = thread.Posts
                .Where(p => p.Text.Contains($"#{post.Id}"))
                .Select(x => x.Id);

            return View(responses);
        }
    }
}
