using Microsoft.AspNetCore.Mvc;
using Bazirano.Models.DataAccess;
using System.Linq;
using Bazirano.Models.News;
using Microsoft.AspNetCore.Authorization;
using Bazirano.Infrastructure;
using System.Threading.Tasks;
using Bazirano.Models.Column;
using System;
using Bazirano.Models.Admin;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Bazirano.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private INewsPostsRepository newsRepo;
        private IBoardThreadsRepository boardRepo;
        private IColumnRepository columnRepo;
        private NewsHelper newsHelper;
        private UserManager<IdentityUser> userManager;

        public AdminController(
            INewsPostsRepository newsRepository,
            IBoardThreadsRepository boardRepository,
            IColumnRepository columnRepository,
            UserManager<IdentityUser> userManager)
        {
            newsRepo = newsRepository;
            boardRepo = boardRepository;
            columnRepo = columnRepository;
            newsHelper = new NewsHelper(newsRepo);
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(nameof(Index));
        }

        public IActionResult News()
        {
            List<NewsPost> newsPosts = newsRepo.NewsPosts.OrderByDescending(x => x.DatePosted).Take(100).ToList();

            return View(nameof(News), newsPosts);
        }

        public IActionResult DeleteArticle(long id)
        {   
            newsRepo.RemoveNewsPost(id);

            return RedirectToAction(nameof(News));
        }

        public IActionResult EditArticle(long id)
        {
            NewsPost post = newsRepo.NewsPosts.FirstOrDefault(x => x.Id == id);

            return View(nameof(EditArticle), post);
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
            var boardThreads = boardRepo.BoardThreads.ToList().SortByBumpOrder();

            return View(nameof(Board), boardThreads);
        }
        
        public IActionResult Column()
        {
            var adminColumnViewModel = new AdminColumnViewModel
            {
                Authors = columnRepo.Authors.ToList(),
                ColumnPosts = columnRepo.ColumnPosts.ToList()
            };

            return View(nameof(Column), adminColumnViewModel);
        }

        public IActionResult AddColumn()
        {
            return View(nameof(AddColumn), new AddColumnViewModel
            {
                Column = new ColumnPost
                {
                    DatePosted = DateTime.Now,
                    Author = new Author()
                },
                Authors = columnRepo.Authors.ToList()
            });
        }

        public IActionResult SaveColumn(ColumnPost column)
        {
            columnRepo.SaveColumn(column);

            return Column();
        }

        public IActionResult DeleteColumn(long id)
        {
            columnRepo.DeleteColumn(id);

            return Column();
        }

        public IActionResult AddAuthor()
        {
            return View(nameof(AddAuthor), new Author());
        }

        public IActionResult SaveAuthor(Author author)
        {
            columnRepo.SaveAuthor(author);

            return Column();
        }

        public IActionResult DeleteAuthor(long id)
        {
            columnRepo.DeleteAuthor(id);

            return Column();
        }

        public IActionResult EditColumn(long id)
        {
            return View(nameof(AddColumn), new AddColumnViewModel
            {
                Column = columnRepo.ColumnPosts.First(p => p.Id == id),
                Authors = columnRepo.Authors.ToList()
            });
        }

        public IActionResult EditAuthor(long id)
        {
            return View(nameof(AddAuthor), columnRepo.Authors.First(a => a.Id == id));
        }

        public IActionResult Accounts()
        {
            var accounts = userManager.Users.ToList();

            return View(nameof(Accounts), accounts);
        }

        public async Task<IActionResult> CreateUser(string userName, string password)
        {
            var user = new IdentityUser(userName);
            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded == false)
            {
                ViewBag.Errors = result.Errors;
            }

            return Accounts();
        }
        public async Task<IActionResult> DeleteUser(string name)
        {
            var user = await userManager.FindByNameAsync(name);

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded == false)
            {
                ViewBag.Errors = result.Errors;
            }

            return Accounts();
        }

    }
}
