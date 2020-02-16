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

namespace Bazirano.Controllers
{
    public class HomeController : Controller
    {
        private IBoardThreadsRepository boardRepo;
        private INewsPostsRepository newsRepo;

        public HomeController(IBoardThreadsRepository boardRepo, INewsPostsRepository newsRepo)
        {
            this.boardRepo = boardRepo;
            this.newsRepo = newsRepo;
        }

        public async Task<IActionResult> Index()
        {
            NewsPageViewModel vm = await newsRepo.GetNewsPageViewModelAsync();

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