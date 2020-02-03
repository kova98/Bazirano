using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bazirano.Models.DataAccess;
using Bazirano.Models.Home;
using Bazirano.Infrastructure;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index()
        {
            var vm = await newsHelper.GetCurrentNewsAsync();

            return View(new HomePageViewModel
            {
                MainPost = vm.MainPost,
                PopularPosts = vm.PostList,
                Threads = await boardRepo.BoardThreads
                    .OrderByDescending(x => x.Posts.OrderBy(y => y.DatePosted).FirstOrDefault().DatePosted)
                    .ToListAsync()
            }); ;
        }
    }
}