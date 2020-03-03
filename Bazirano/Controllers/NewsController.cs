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
        private IGoogleRecaptchaHelper googleRecaptchaHelper;
        private NewsHelper newsHelper;

        public NewsController(INewsPostsRepository repo, IGoogleRecaptchaHelper googleRecaptchaHelper)
        {
            newsRepo = repo;
            newsHelper = new NewsHelper(repo);
            this.googleRecaptchaHelper = googleRecaptchaHelper;
        }

        [Route("~/vijesti")]
        public IActionResult Index()
        {
            return View(nameof(Index), newsHelper.GetNewsPageViewModel());
        }

        [Route("~/vijesti/clanak/{id}")]
        public IActionResult Article(long id)
        {
            var article = newsRepo.NewsPosts.FirstOrDefault(p => p.Id == id);
            var newsPageViewModel = newsHelper.GetNewsPageViewModel();

            if (article == null)
            {
                return RedirectToAction("Article", "Error");
            }

            newsRepo.IncrementViewCount(article);

            ArticleViewModel vm = new ArticleViewModel
            {
                Article = article,
                LatestNews = newsPageViewModel.LatestPosts
            };

            return View(nameof(Article), vm);
        }

        public async Task <IActionResult> PostComment(ArticleRespondViewModel vm)
        {
            var article = newsRepo.NewsPosts.FirstOrDefault(p => p.Id == vm.ArticleId);

            if (article == null)
            {
                RedirectToAction("Article", "Error");
            }

            await VerifyRecaptcha();

            if (ModelState.IsValid)
            {
                vm.Comment.DatePosted = DateTime.Now;

                if (string.IsNullOrEmpty(vm.Comment.Username))
                {
                    vm.Comment.Username = "Anonimac";
                }

                vm.Comment.Text = vm.Comment.Text.Trim();
                
                newsRepo.AddCommentToNewsPost(vm.Comment, vm.ArticleId);

                ModelState.Clear();

                ViewBag.CommentPosted = true;
            }

            return Article(vm.ArticleId);
        }

        private async Task VerifyRecaptcha()
        {
            if (Request != null) // Only when unit testing
            {
                if (!await googleRecaptchaHelper.IsRecaptchaValid(Request.Form["g-recaptcha-response"]))
                {
                    ModelState.AddModelError("captchaError", "CAPTCHA provjera neispravna.");
                }
            }
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