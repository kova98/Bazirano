using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Infrastructure;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
using Bazirano.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Bazirano.Controllers
{
    public class NewsController : Controller
    {
        private INewsPostsRepository newsRepo;
        private IConfiguration config;
        private NewsHelper newsHelper;

        public NewsController(INewsPostsRepository repo, IConfiguration cfg)
        {
            newsRepo = repo;
            config = cfg;
            newsHelper = new NewsHelper(repo);
        }

        [Route("~/vijesti")]
        public IActionResult Index()
        {
            return View(newsHelper.GetNewsPageViewModel());
        }

        [Route("~/vijesti/clanak/{id}")]
        public IActionResult Article(long id)
        {
            NewsPost article = newsRepo.NewsPosts.FirstOrDefault(p => p.Id == id);
            var newsPageVm = newsHelper.GetNewsPageViewModel();
            ArticleViewModel vm = new ArticleViewModel
            {
                Article = article,
                LatestNews = newsPageVm.LatestPosts
            };

            newsRepo.IncrementViewCount(article);

            return View(vm);
        }

        public IActionResult PostComment(ArticleRespondViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Comment.DatePosted = DateTime.Now;

                if (string.IsNullOrEmpty(vm.Comment.Username))
                {
                    vm.Comment.Username = "Anonimac";
                }

                vm.Comment.Text = vm.Comment.Text.Trim();

                newsRepo.AddCommentToNewsPost(vm.Comment, vm.ArticleId);
            }

            var newsPageViewModel = newsHelper.GetNewsPageViewModel();

            var articleViewModel = new ArticleViewModel
            {
                Article = newsRepo.NewsPosts.FirstOrDefault(p => p.Id == vm.ArticleId),
                LatestNews = newsPageViewModel.LatestPosts,
            };

            ViewBag.CommentPosted = true;


            return View(nameof(Article), articleViewModel);
        }

        //TODO: Add security!!!
        [HttpPost("~/api/postNews")]
        public IActionResult PostNews([FromBody]NewsPost post)
        {
            if (ModelState.IsValid)
            {
                var latestPosts = newsRepo.NewsPosts.OrderByDescending(x => x.DatePosted).Take(15).ToList();

                foreach (var p in latestPosts)
                {
                    if (p.Guid == post.Guid)
                    {
                        newsRepo.EditNewsPost(post);
                        return Ok();
                    }
                }

                post.DatePosted = DateTime.Now;
                post.Comments = new List<Comment>();

                newsRepo.AddNewsPost(post);

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        
    }
}