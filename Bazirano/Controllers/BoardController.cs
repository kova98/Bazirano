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
        private IGoogleRecaptchaHelper googleRecaptchaHelper;
        private IWriter writer;

        // TODO: Make this configurable through the admin panel
        private int maxThreadCount = 40;
        private int maxImagesInThread = 50;

        public BoardController(IBoardThreadsRepository repo, IGoogleRecaptchaHelper grHelper, IWriter writer)
        {
            repository = repo;
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
                    await writer.UploadImage(vm.BoardPost, file);
                }

                vm.BoardPost.Text = vm.BoardPost.Text.Trim();
                repository.AddPostToThread(vm.BoardPost, vm.ThreadId);
                ModelState.Clear();
            }

            return Thread(thread.Id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateThread(BoardPost post, IFormFile file)
        {
            await googleRecaptchaHelper.VerifyRecaptcha(Request, ModelState);

            if (!ModelState.IsValid)
            {
                return View(nameof(Submit));
            }

            post.Text = post.Text.Trim();
            post.DatePosted = DateTime.Now;

            if (IsImageFileValid(file))
            {
                await writer.UploadImage(post, file);
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

            long threadId = repository.BoardThreads.First(t => t.Posts.Contains(post)).Id;

            return Thread(threadId);
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
            int threadCount = repository.BoardThreads.Count();
            BoardThread lastThread = repository.BoardThreads.ToList().SortByBumpOrder().Last();

            if (threadCount > maxThreadCount)
            {
                repository.RemoveThread(lastThread);
            }
        }
    }
}