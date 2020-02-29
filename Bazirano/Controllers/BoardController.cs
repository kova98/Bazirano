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

namespace Bazirano.Controllers
{
    public class BoardController : Controller
    {
        private IBoardThreadsRepository repository;
        private IConfiguration config;
        private IGoogleRecaptchaHelper googleRecaptchaHelper;
        private IWriter writer;

        // TODO: Make this configurable through the admin panel
        private int maxThreadCount = 40;
        private int maxImagesInThread = 50;

        public BoardController(IBoardThreadsRepository repo, IConfiguration cfg, IGoogleRecaptchaHelper grHelper, IWriter writer)
        {
            repository = repo;
            config = cfg;
            googleRecaptchaHelper = grHelper;
            this.writer = writer;
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

            return View(nameof(Thread), thread);
        }

        public async Task<IActionResult> Respond(BoardRespondViewModel vm, IFormFile file)
        {
            var thread = repository.BoardThreads.FirstOrDefault(t => t.Id == vm.ThreadId);

            await VerifyRecaptcha();

            if (MaxImagesCountReached(thread))
            {
                ModelState.AddModelError("maxImgCountError", "Maksimalni broj slika u dretvi premašen.");
            }

            await RespondToThread(vm, file);

            return View(nameof(Thread), thread);
        }

        private async Task RespondToThread(BoardRespondViewModel vm, IFormFile file)
        {
            if (file != null && IsImageFileValid(file))
            {
                await writer.UploadImage(vm.BoardPost, file);
            }

            if (ModelState.IsValid && ModelState.Count > 0)
            {
                vm.BoardPost.Text = vm.BoardPost.Text.Trim();
                repository.AddPostToThread(vm.BoardPost, vm.ThreadId);
                TempData["NewPost"] = vm.BoardPost.Id;
                ModelState.Clear();
            }
        }

        private bool MaxImagesCountReached(BoardThread thread)
        {
            return thread.ImageCount > maxImagesInThread;
        }

        private async Task VerifyRecaptcha()
        {
            if (!await googleRecaptchaHelper.IsRecaptchaValid(Request.Form["g-recaptcha-response"], config["GoogleReCaptcha:secret"]))
            {
                ModelState.AddModelError("captchaError", "CAPTCHA provjera neispravna.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateThread(BoardPost post, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Submit));
            }

            if (!await googleRecaptchaHelper.IsRecaptchaValid(Request.Form["g-recaptcha-response"], config["GoogleReCaptcha:secret"]))
            {
                ViewBag.CaptchaError = "CAPTCHA provjera neispravna.";
                return View(nameof(Submit));
            }

            if (file != null)
            {
                if (IsImageFileValid(file))
                {
                    await writer.UploadImage(post, file);
                }
                else
                {
                    return View(nameof(Submit));
                }
            }

            post.Text = post.Text.Trim();
            post.DatePosted = DateTime.Now;

            BoardThread thread = new BoardThread
            {
                PostCount = 0,
                ImageCount = file == null ? 0 : 1,
                IsLocked = false,
                Posts = new List<BoardPost> { post }
            };

            repository.AddThread(thread);
            PruneLastThread();

            return RedirectToAction(nameof(Thread), new { post.Id });
        }

        private bool IsImageFileValid(IFormFile file)
        {
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

        private void PruneLastThread()
        {
            // TODO: Change this to use a list with threads sorted by bump order. 
            // Implement bump order sorting.

            int threadCount = repository.BoardThreads.Count();

            if (threadCount > maxThreadCount)
            {
                BoardThread oldestThread = repository.BoardThreads.FirstOrDefault();

                // Find the thread with the oldest recent post.
                foreach (var thread in repository.BoardThreads)
                {
                    var newestPost = thread.Posts.OrderByDescending(x => x.DatePosted).FirstOrDefault();
                    var newestPostFromOldestThread = oldestThread.Posts.OrderByDescending(x => x.DatePosted).FirstOrDefault();
                    if (newestPost.DatePosted < newestPostFromOldestThread.DatePosted) // newestPost is older
                    {
                        oldestThread = thread;
                    }
                }

                repository.RemoveThread(oldestThread);
            }
        }
    }
}