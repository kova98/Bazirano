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

        // TODO: Make this configurable through the admin panel
        private int maxThreadCount = 20;

        public BoardController(IBoardThreadsRepository repo, IConfiguration cfg)
        {
            repository = repo;
            config = cfg;
        }

        [Route("~/ploca/objavi")]
        public IActionResult Submit()
        {
            return View();
        }

        [Route("~/ploca")]
        public IActionResult Catalog()
        {
            return View(repository.BoardThreads
                .OrderByDescending(t => t.Id)
                .ToList());
        }

        [Route("~/ploca/dretva/{id}")]
        public IActionResult Thread(long id)
        {
            return View(repository.BoardThreads
                    .FirstOrDefault(t => t.Posts.FirstOrDefault().Id == id));
        }

        public async Task<IActionResult> Respond(BoardRespondViewModel vm, IFormFile file)
        {
            var thread = repository.BoardThreads
                    .FirstOrDefault(t => t.Id == vm.ThreadId);

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    if (!file.ContentType.StartsWith("image"))
                    {
                        ViewBag.FileError = "Nepodržan format datoteke.";
                        return View(nameof(Thread), thread);
                    }

                    if (WriterHelper.ByteToMegabyte(file.Length) > 5)
                    {
                        ViewBag.FileError = "Datoteka je prevelika.";
                        return View(nameof(Thread), thread);
                    }

                    await WriterHelper.UploadImage(vm.BoardPost, file);
                }
                repository.AddPostToThread(vm.BoardPost, vm.ThreadId);
            }

            TempData["NewPost"] = vm.BoardPost.Id;
            return RedirectToAction(nameof(Thread), new { thread.Posts.FirstOrDefault().Id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateThread(BoardPost post, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Submit));
            }

            if (!await GoogleRecaptchaHelper.IsReCaptchaPassedAsync(Request.Form["g-recaptcha-response"], config["GoogleReCaptcha:secret"]))
            {
                ViewBag.CaptchaError = "CAPTCHA provjera neispravna.";
                return View(nameof(Submit));
            }

            post.DatePosted = DateTime.Now;

            BoardThread thread = new BoardThread
            {
                PostCount = 0,
                ImageCount = file == null ? 0 : 1,
                IsLocked = false,
                Posts = new List<BoardPost> { post }
            };

            if (file != null)
            {
                if (!file.ContentType.StartsWith("image"))
                {
                    ViewBag.FileError = "Nepodržan format datoteke.";
                    return View(nameof(Submit));
                }

                if (WriterHelper.ByteToMegabyte(file.Length) > 5)
                {
                    ViewBag.FileError = "Datoteka je prevelika.";
                    return View(nameof(Submit));
                }

                await WriterHelper.UploadImage(post, file);
            }

            repository.AddThread(thread);
            PruneLastThread();

            return RedirectToAction(nameof(Thread), new { post.Id });
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