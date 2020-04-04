using Bazirano.Models.Board;
using Bazirano.Models.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Bazirano.Components
{
    public class PostResponsesViewComponent : ViewComponent
    {
        private IBoardThreadRepository repository;

        public PostResponsesViewComponent(IBoardThreadRepository repo)
        {
            repository = repo;
        }

        public IViewComponentResult Invoke(BoardPostViewModel viewModel)
        {
            var responses = viewModel.ParentThread.Posts
                .Where(p => p.Text.Contains($"#{viewModel.Post.Id}"))
                .Select(x => x.Id);

            return View(responses);
        }
    }
}

