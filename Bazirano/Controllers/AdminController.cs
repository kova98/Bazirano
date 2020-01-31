using Microsoft.AspNetCore.Mvc;
using Bazirano.Models.DataAccess;
using System.Linq;
using Bazirano.Models.News;
using Microsoft.AspNetCore.Authorization;
using Bazirano.Infrastructure;
using System.Threading.Tasks;

namespace Bazirano.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public INewsPostsRepository newsRepo;
        public IBoardThreadsRepository boardRepo;
        public INewsHelper newsHelper;

        public AdminController(INewsPostsRepository newsRepository, IBoardThreadsRepository boardRepository, INewsHelper helper)
        {
            newsRepo = newsRepository;
            boardRepo = boardRepository;
            newsHelper = helper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> News()
        {
            var newsPosts = await newsRepo.GetLatestNewsPosts(100);
            return View(nameof(News), newsPosts);
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
                .Take(50));
        }
    }
}
