using Microsoft.AspNetCore.Mvc;
using Bazirano.Models.DataAccess;
using System.Linq;
using Bazirano.Models.News;
using Microsoft.AspNetCore.Authorization;

namespace Bazirano.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public INewsPostsRepository newsRepo;
        public IBoardThreadsRepository boardRepo;

        public AdminController(INewsPostsRepository newsRepository, IBoardThreadsRepository boardRepository)
        {
            newsRepo = newsRepository;
            boardRepo = boardRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult News()
        {
            return View(nameof(News), newsRepo.NewsPosts.OrderByDescending(x=>x.DatePosted).ToList().Take(100));
        }

        public IActionResult DeleteArticle(long id)
        {
            newsRepo.RemoveNewsPost(newsRepo.NewsPosts.FirstOrDefault(x => x.Id == id));

            return RedirectToAction(nameof(News));
        }

        public IActionResult EditArticle(long id)
        {
            NewsPost post = newsRepo.NewsPosts.FirstOrDefault(x => x.Id == id);

            return View(post);
        }

        public IActionResult SaveArticle(NewsPost article)
        {
            newsRepo.EditNewsPost(article);

            return RedirectToAction(nameof(News));
        }

        public IActionResult DeleteBoardThread(long id)
        {
            boardRepo.RemoveThread(boardRepo.BoardThreads.FirstOrDefault(x => x.Id == id));

            return RedirectToAction(nameof(Board));
        }

        public IActionResult Board()
        {
            return View(nameof(Board), boardRepo.BoardThreads
                .OrderByDescending(x => x.Posts.FirstOrDefault().DatePosted)
                .ToList()
                .Take(100));
        }
    }
}
