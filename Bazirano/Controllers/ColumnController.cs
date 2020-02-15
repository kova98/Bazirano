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

            // Should only happen when first launching the website, without any columns in the database
            if (columns.Count == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    columns.Add(new ColumnPost
                    {
                        Author = new Author { Id = 0, Name = "Dummy", Image = "/", Bio = "Dummy bio" },
                        Id = 0,
                        Title = $"Dummy Column {i + 1}",
                        Text = $"Dummy column text {i + 1}"
                    });
                }
            }

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

        [Route("~/kolumnist/{id}")]
        public IActionResult Author(long id)
        {
            var author = columnRepo.Authors.First(p => p.Id == id);
            var columns = columnRepo.ColumnPosts
                .Where(p => p.Author.Id == id)
                .OrderByDescending(p => p.DatePosted)
                .ToList();

            return View(new AuthorPageViewModel
            {
                Author = author,
                Columns = columns
            });
        }
    }
}