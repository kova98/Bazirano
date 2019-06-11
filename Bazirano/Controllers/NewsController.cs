using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Infrastructure;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
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

        //TODO: Add security!!!
        [HttpPost("~/api/postNews")]
        public IActionResult PostNews([FromBody]NewsPost post)
        {
            if (ModelState.IsValid)
            {
                post.DatePosted = DateTime.Now;
                post.Comments = new List<NewsComment>();

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