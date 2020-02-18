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

namespace Bazirano.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private INewsPostsRepository newsRepo;
        private IBoardThreadsRepository boardRepo;
        private IColumnRepository columnRepo;
        private NewsHelper newsHelper;

        public AdminController(INewsPostsRepository newsRepository, IBoardThreadsRepository boardRepository, IColumnRepository columnRepository)
        {
            newsRepo = newsRepository;
            boardRepo = boardRepository;
            columnRepo = columnRepository;
            newsHelper = new NewsHelper(newsRepo);
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
            newsRepo.RemoveNewsPost(newsRepo.NewsPosts.FirstOrDefault(x => x.Id == id));

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
            var boardThreads = boardRepo.BoardThreads
                .OrderByDescending(x => x.Posts.FirstOrDefault().DatePosted)
                .ToList();

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
                    DatePosted = DateTime.Now
                },
                Authors = columnRepo.Authors.ToList()
            });
        }

        public IActionResult SaveColumn(ColumnPost column)
        {
            columnRepo.SaveColumn(column);

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
    }
}
