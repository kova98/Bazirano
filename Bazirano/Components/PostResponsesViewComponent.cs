using Bazirano.Models.Board;
using Bazirano.Models.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
