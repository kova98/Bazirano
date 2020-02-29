using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Infrastructure;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Bazirano.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Bazirano.Controllers
{
    public class ColumnController : Controller
    {
        private IColumnRepository columnRepo;
        private IGoogleRecaptchaHelper googleRecaptchaHelper;
        private IConfiguration config;

        public ColumnController(IColumnRepository columnRepository, IGoogleRecaptchaHelper googleRecaptchaHelper, IConfiguration config)
        {
            columnRepo = columnRepository;
            this.googleRecaptchaHelper = googleRecaptchaHelper;
            this.config = config;
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
                columns.AddRange(GetDummyColumns(6));
            }

            return View(nameof(Index), new ColumnMainPageViewModel
            {
                FirstColumn = columns.First(),
                Authors = columnRepo.Authors.ToList(),
                Columns = columns.GetRange(1, columns.Count - 1)
            });
        }

        private List<ColumnPost> GetDummyColumns(int amount)
        {
            List<ColumnPost> columnPosts = new List<ColumnPost>();

            for (int i = 0; i < amount; i++)
            {
                columnPosts.Add(new ColumnPost
                {
                    Author = new Author { Id = 0, Name = "Dummy", Image = "/", Bio = $"Dummy bio {i}" },
                    Id = 0,
                    Title = $"Dummy Column {i + 1}",
                    Text = $"Dummy column text {i + 1}"
                });
            }

            return columnPosts;
        }

        [Route("~/kolumna/{id}")]
        public IActionResult ColumnPost(long id)
        {
            var post = columnRepo.ColumnPosts.First(p => p.Id == id);
            if (post.Comments == null)
            {
                post.Comments = new List<Comment>();
            }
            return View(nameof(ColumnPost), post);
        }

        [Route("~/kolumnist/{id}")]
        public IActionResult Author(long id)
        {
            var author = columnRepo.Authors.FirstOrDefault(p => p.Id == id);
            var columns = columnRepo.ColumnPosts
                .Where(p => p.Author.Id == id)
                .OrderByDescending(p => p.DatePosted)
                .ToList();

            return View(nameof(Author), new AuthorPageViewModel
            {
                Author = author,
                Columns = columns
            });
        }

        public async Task<IActionResult> PostComment(ColumnRespondViewModel viewModel)
        {
            await VerifyRecaptcha();

            if (ModelState.IsValid)
            {
                viewModel.Comment.DatePosted = DateTime.Now;

                if (string.IsNullOrEmpty(viewModel.Comment.Username))
                {
                    viewModel.Comment.Username = "Anonimac";
                }

                viewModel.Comment.Text = viewModel.Comment.Text.Trim();

                columnRepo.AddCommentToColumn(viewModel.Comment, viewModel.ColumnId);

                ModelState.Clear();
            }

            ViewBag.CommentPosted = true;

            return ColumnPost(viewModel.ColumnId);
        }

        public async Task<IActionResult> RespondToComment(CommentRespondViewModel viewModel)
        {
            await VerifyRecaptcha();

            if (ModelState.IsValid && ModelState.Count > 0)
            {
                viewModel.Comment.DatePosted = DateTime.Now;

                if (string.IsNullOrEmpty(viewModel.Comment.Username))
                {
                    viewModel.Comment.Username = "Anonimac";
                }

                viewModel.Comment.Text = viewModel.Comment.Text.Trim();

                columnRepo.AddCommentResponse(viewModel.Comment, viewModel.CommentId);

                ModelState.Clear();
            }

            ViewBag.CommentPosted = true;

            return Redirect(viewModel.ReturnUrl);
        }

        private async Task VerifyRecaptcha()
        {
            if (!await googleRecaptchaHelper.IsRecaptchaValid(Request.Form["g-recaptcha-response"], config["GoogleReCaptcha:secret"]))
            {
                ModelState.AddModelError("captchaError", "CAPTCHA provjera neispravna.");
            }
        }
    }
}