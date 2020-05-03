using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Infrastructure;
using Bazirano.Models.Board;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
using Bazirano.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Bazirano.Controllers
{
    public class NewsController : Controller
    {
        private IArticleRepository articleRepo;
        private IGoogleRecaptchaHelper googleRecaptchaHelper;
        private NewsHelper newsHelper;

        public NewsController(IArticleRepository articleRepo, IGoogleRecaptchaHelper googleRecaptchaHelper)
        {
            this.googleRecaptchaHelper = googleRecaptchaHelper;
            this.articleRepo = articleRepo;
            newsHelper = new NewsHelper(articleRepo);
        }

        [Route("~/vijesti")]
        public IActionResult Index()
        {
            return View(nameof(Index), newsHelper.GetNewsPageViewModel());
        }

        [Route("~/vijesti/clanak/{id}")]
        public IActionResult Article(long id)
        {
            var article = articleRepo.Articles.FirstOrDefault(p => p.Id == id);
            var newsPageViewModel = newsHelper.GetNewsPageViewModel();

            if (article == null)
            {
                return RedirectToAction("Article", "Error");
            }

            articleRepo.IncrementViewCount(article);

            ArticleViewModel vm = new ArticleViewModel
            {
                Article = article,
                LatestNews = newsPageViewModel.LatestPosts
            };

            return View(nameof(Article), vm);
        }

        //TODO: Add security!!!
        [HttpPost("~/api/postNews")]
        public IActionResult PostNews([FromBody]Article post)
        {
            if (ModelState.IsValid)
            {
                var latestPosts = articleRepo.Articles.OrderByDescending(x => x.DatePosted).Take(15).ToList();

                foreach (var p in latestPosts)
                {
                    if (p.Guid == post.Guid)
                    {
                        post.Id = p.Id;
                        articleRepo.EditArticle(post);
                        return Ok();
                    }
                }

                post.DatePosted = DateTime.Now;

                articleRepo.AddArticle(post);

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}