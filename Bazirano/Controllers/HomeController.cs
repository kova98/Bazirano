using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bazirano.Models.DataAccess;
using Bazirano.Models.Home;
using Bazirano.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Bazirano.Models.News;
using Bazirano.Models.Board;

namespace Bazirano.Controllers
{
    public class HomeController : Controller
    {
        private IBoardThreadRepository boardRepo;
        private IArticleRepository newsRepo;
        private IColumnRepository columnRepo;
        private NewsHelper newsHelper;

        public HomeController(IBoardThreadRepository boardRepo, IArticleRepository newsRepo, IColumnRepository columnRepo)
        {
            this.boardRepo = boardRepo;
            this.newsRepo = newsRepo;
            this.columnRepo = columnRepo;
            newsHelper = new NewsHelper(newsRepo);
        }

        public IActionResult Index()
        {
            var newsPageViewModel = newsHelper.GetNewsPageViewModel();
            var boardThreads = boardRepo.BoardThreads
                .Where(t=>t.SafeForWork == true)
                .ToList()
                .SortByBumpOrder();
            var columnPosts = columnRepo.ColumnPosts
                .OrderByDescending(p => p.DatePosted)
                .Take(5)
                .ToList();

            return View(nameof(Index), new HomePageViewModel
            {
                MainPost = newsPageViewModel.MainPost,
                PopularPosts = newsPageViewModel.PostList,
                Threads = boardThreads,
                ColumnPosts = columnPosts
            });
        }
    }
}