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
        private INewsPostsRepository newsRepo;
        private NewsHelper newsHelper;

        public HomeController(IBoardThreadsRepository boardRepo, INewsPostsRepository newsRepo)
        {
            this.boardRepo = boardRepo;
            this.newsRepo = newsRepo;
            newsHelper = new NewsHelper(newsRepo);
        }

        public IActionResult Index()
        {
            NewsPageViewModel newsPageViewModel = newsHelper.GetNewsPageViewModel();
            List<BoardThread> boardThreads = boardRepo.BoardThreads
                .OrderByDescending(x => x.Posts.OrderBy(y => y.DatePosted).FirstOrDefault().DatePosted)
                .ToList();

            return View(new HomePageViewModel
            {
                MainPost = newsPageViewModel.MainPost,
                PopularPosts = newsPageViewModel.PostList,
                Threads = boardThreads
            });
        }
    }
}