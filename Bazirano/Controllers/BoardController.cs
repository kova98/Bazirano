using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Infrastructure;
using Bazirano.Models.Board;
using Bazirano.Models.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bazirano.Controllers
{
    public class BoardController : Controller
    {
        private readonly IBoardThreadRepository repository;
        private readonly IGoogleRecaptchaHelper googleRecaptchaHelper;
        private readonly IWriter writer;
        private readonly ILogger<BoardController> logger;

        // TODO: Make this configurable through the admin panel
        private readonly int maxThreadCount = 40;
        private readonly int maxImagesInThread = 50;

        public BoardController(
            IBoardThreadRepository repo,
            IGoogleRecaptchaHelper grHelper,
            IWriter writer,
            ILogger<BoardController> logger)
        {
            repository = repo;
            googleRecaptchaHelper = grHelper;
            this.writer = writer;
            this.logger = logger;
        }

        [Route("~/ploca/objavi")]
        public IActionResult Submit()
        {
            return View(nameof(Submit));
        }

        [Route("~/ploca")]
        public IActionResult Catalog()
        {
            var threads = repository.BoardThreads.ToList().SortByBumpOrder();

            return View(nameof(Catalog), threads);
        }

        [Route("~/ploca/dretva/{id}")]
        public IActionResult Thread(long id)
        {
            var thread = repository.BoardThreads.FirstOrDefault(t => t.Id == id);

            if (thread == null)
            {
                return RedirectToAction("Thread", "Error");
            }

            return View(nameof(Thread), thread);
        }

        public async Task<IActionResult> Respond(BoardRespondViewModel vm, IFormFile file)
        {
            var thread = repository.BoardThreads.FirstOrDefault(t => t.Id == vm.ThreadId);

            await googleRecaptchaHelper.VerifyRecaptcha(Request, ModelState);

            if (MaxImagesCountReached(thread))
            {
                ModelState.AddModelError("maxImgCountError", "Maksimalni broj slika u dretvi premašen.");
            }

            if (ModelState.IsValid)
            {
                if (IsImageFileValid(file))
                {
                    vm.BoardPost.Image = await writer.UploadImage(file);
                }

                vm.BoardPost.Text = vm.BoardPost.Text.Trim();
                repository.AddPostToThread(vm.BoardPost, vm.ThreadId);
                ModelState.Clear();
                TempData["ScrollToComment"] = true;
                return RedirectToAction("Thread", "Board", new { thread.Id });
            }
            else
            {
                return Thread(thread.Id);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateThread(SubmitViewModel vm, IFormFile file)
        {
            await googleRecaptchaHelper.VerifyRecaptcha(Request, ModelState);

            if (!ModelState.IsValid)
            {
                return Submit();
            }

            var post = new BoardPost
            {
                Text = vm.Text.Trim(),
                DatePosted = DateTime.Now
            };

            if (string.IsNullOrEmpty(vm.ImageUrl) == false)
            {
                try
                {
                    post.Image = await writer.DownloadImageFromUrl(vm.ImageUrl);
                }
                catch
                {
                    logger.LogError("An error occured while trying to download image from " + vm.ImageUrl);
                    ModelState.AddModelError("ImageUrl", "Došlo je do greške. Provjerite ispravnost poveznice.");
                    return Submit();
                }
            }
            else if (IsImageFileValid(file))
            {
                post.Image = await writer.UploadImage(file);
            }

            var thread = new BoardThread
            {
                PostCount = 0,
                ImageCount = file == null ? 0 : 1,
                IsLocked = false,
                Posts = new List<BoardPost> { post }
            };

            repository.AddThread(thread);
            RemoveLastThread();

            return RedirectToAction("Thread", "Board", new { thread.Id });
        }

        private bool MaxImagesCountReached(BoardThread thread)
        {
            return thread.ImageCount > maxImagesInThread;
        }

        private bool IsImageFileValid(IFormFile file)
        {
            if (file == null)
            {
                return false;
            }

            if (!file.ContentType.StartsWith("image"))
            {
                ModelState.AddModelError("", "Nepodržan format datoteke.");
                return false;
            }

            if (writer.ByteToMegabyte(file.Length) > 5)
            {
                ModelState.AddModelError("", "Datoteka je prevelika.");
                return false;
            }

            return true;
        }

        private void RemoveLastThread()
        {
            var threadCount = repository.BoardThreads.Count();
            var lastThread = repository.BoardThreads.ToList().SortByBumpOrder().LastOrDefault();

            if (threadCount > maxThreadCount)
            {
                repository.RemoveThread(lastThread.Id);
            }
        }
    }
}