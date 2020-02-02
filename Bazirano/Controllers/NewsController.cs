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
        private INewsHelper helper;
        private INewsPostsRepository repository;
        private IConfiguration config;

        public NewsController(INewsHelper helper, INewsPostsRepository repo, IConfiguration cfg)
        {
            this.helper = helper;
            repository = repo;
            config = cfg;
        }

        [Route("~/vijesti")]
        public async Task<IActionResult> Index()
        {
            return View(await helper.GetCurrentNewsAsync());
        }

        [Route("~/vijesti/clanak/{id}")]
        public async Task<IActionResult> Article(long id)
        {
            NewsPost article = repository.NewsPosts.FirstOrDefault(p => p.Id == id);
            var newsPageVm = await helper.GetCurrentNewsAsync();
            ArticleViewModel vm = new ArticleViewModel
            {
                Article = article,
                LatestNews = newsPageVm.LatestNews
            };

            repository.IncrementViewCount(article);

            return View(vm);
        }

        public async Task<IActionResult> PostComment(ArticleRespondViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Comment.DatePosted = DateTime.Now;

                if (string.IsNullOrEmpty(vm.Comment.Username))
                {
                    vm.Comment.Username = "Anonimac";
                }

                vm.Comment.Text = vm.Comment.Text.Trim();

                repository.AddCommentToNewsPost(vm.Comment, vm.ArticleId);
            }

            var newsPageVm = await helper.GetCurrentNewsAsync();

            var articleVm = new ArticleViewModel
            {
                Article = repository.NewsPosts.FirstOrDefault(p => p.Id == vm.ArticleId),
                LatestNews = newsPageVm.LatestNews,
            };

            ViewBag.CommentPosted = true;


            return View(nameof(Article), articleVm);
        }

        //TODO: Add security!!!
        [HttpPost("~/api/postNews")]
        public async Task<IActionResult> PostNews([FromBody]NewsPost post)
        {
            if (ModelState.IsValid)
            {
                var latestPosts = await repository.GetLatestNewsPosts(15);

                foreach(var p in latestPosts)
                {
                    if (p.Guid == post.Guid)
                    {
                        repository.EditNewsPost(post);
                        return Ok();
                    }
                }

                post.DatePosted = DateTime.Now;
                post.Comments = new List<Comment>();

                repository.AddNewsPost(post);

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}