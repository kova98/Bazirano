using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bazirano.Models.DataAccess;
using Bazirano.Models.Home;
using Bazirano.Infrastructure;

namespace Bazirano.Controllers
{
    public class HomeController : Controller
    {
        private IBoardThreadsRepository boardRepo;
        private INewsHelper newsHelper;

        public HomeController(IBoardThreadsRepository boardRepo, INewsHelper newsHelper)
        {
            this.boardRepo = boardRepo;
            this.newsHelper = newsHelper;
        }

        public IActionResult Index()
        {
            return View(new HomePageViewModel
            {
                MainPost = newsHelper.CurrentNews.MainPost,
                PopularPosts = newsHelper.CurrentNews.PostList,
                Threads = boardRepo.BoardThreads.Take(6).ToList()
            });
        }
    }
}