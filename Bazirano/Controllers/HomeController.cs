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
        private IBoardThreadsRepository boardRepo;
        private IArticleRepository newsRepo;
        private NewsHelper newsHelper;

        public HomeController(IBoardThreadsRepository boardRepo, IArticleRepository newsRepo)
        {
            this.boardRepo = boardRepo;
            this.newsRepo = newsRepo;
            newsHelper = new NewsHelper(newsRepo);
        }

        public IActionResult Index()
        {
            NewsPageViewModel newsPageViewModel = newsHelper.GetNewsPageViewModel();
            List<BoardThread> boardThreads = boardRepo.BoardThreads.ToList().SortByBumpOrder();

            return View(nameof(Index), new HomePageViewModel
            {
                MainPost = newsPageViewModel.MainPost,
                PopularPosts = newsPageViewModel.PostList,
                Threads = boardThreads
            });
        }
    }
}