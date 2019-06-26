using Microsoft.AspNetCore.Mvc;
using Bazirano.Models.DataAccess;
using System.Linq;

namespace Bazirano.Controllers
{
    public class AdminController : Controller
    {
        public INewsPostsRepository repo;

        public AdminController(INewsPostsRepository repository)
        {
            repo = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult News()
        {
            return View(nameof(News), repo.NewsPosts.OrderByDescending(x=>x.DatePosted).ToList().Take(100));
        }

        public IActionResult Delete(long id)
        {
            repo.RemoveNewsPost(repo.NewsPosts.First(x => x.Id == id));

            return RedirectToAction(nameof(News));
        }
    }
}
