using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Infrastructure;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
using Bazirano.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Bazirano.Controllers
{
    public class NewsController : Controller
    {
        private INewsHelper helper;
        private INewsPostsRepository repository;

        public NewsController(INewsHelper helper, INewsPostsRepository repo)
        {
            this.helper = helper;
            repository = repo;
        }

        public IActionResult Index()
        {
            return View(helper.CurrentNews);
        }

        public IActionResult Article(long id)
        {
            NewsPost article = repository.NewsPosts.FirstOrDefault(p => p.Id == id);
            ArticleViewModel vm = new ArticleViewModel
            {
                Article = article,
                LatestNews = helper.CurrentNews.LatestNews
            };

            repository.IncrementViewCount(article);

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

                repository.AddCommentToNewsPost(vm.Comment, vm.ArticleId);
            }

            var articleVm = new ArticleViewModel
            {
                Article = repository.NewsPosts.FirstOrDefault(p => p.Id == vm.ArticleId),
                LatestNews = helper.CurrentNews.LatestNews,
            };

            ViewBag.CommentPosted = true;

            return View(nameof(Article), articleVm);
        }

        //TODO: Add security!!!
        [HttpPost("~/api/postNews")]
        public IActionResult PostNews([FromBody]NewsPost post)
        {
            if (ModelState.IsValid)
            {
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