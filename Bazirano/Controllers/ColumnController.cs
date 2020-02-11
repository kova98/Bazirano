using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Bazirano.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Bazirano.Controllers
{
    public class ColumnController : Controller
    {
        private IColumnRepository columnRepo;

        public ColumnController(IColumnRepository columnRepository)
        {
            columnRepo = columnRepository;
        }

        [Route("~/kolumna")]
        public IActionResult Index()
        {
            var columns = columnRepo.ColumnPosts
                .OrderByDescending(p => p.DatePosted)
                .ToList();

            return View(new ColumnMainPageViewModel
            {
                FirstColumn = columns.First(),
                Authors = columnRepo.Authors.ToList(),
                Columns = columns.GetRange(1, columns.Count - 1)
            });
        }

        [Route("~/kolumna/{id}")]
        public IActionResult ColumnPost(long id)
        {
            var post = columnRepo.ColumnPosts.First(p => p.Id == id);
            if (post.Comments == null)
            {
                post.Comments = new List<Comment>();
            }
            return View(post);
        }
    }
}