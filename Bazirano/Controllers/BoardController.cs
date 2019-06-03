using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Infrastructure;
using Bazirano.Models;
using Bazirano.Models.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bazirano.Controllers
{
    public class BoardController : Controller
    {
        private IBoardThreadsRepository repository;
        public BoardController(IBoardThreadsRepository repo)
        {
            repository = repo;
        }

        public IActionResult Submit()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View(repository.BoardThreads
                .OrderByDescending(t => t.Id)
                .ToList());
        }

        public IActionResult Thread(long id)
        {
            return View(repository.BoardThreads
                    .FirstOrDefault(t => t.Posts.First().Id == id));
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

            return View(nameof(Thread), thread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateThread(BoardPost post, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return View("Submit");
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
                    return View("Submit");
                }

                if (WriterHelper.ByteToMegabyte(file.Length) > 5)
                {
                    ViewBag.FileError = "Datoteka je prevelika.";
                    return View("Submit");
                }

                await WriterHelper.UploadImage(post, file);
            }

            repository.AddThread(thread);

            return RedirectToAction(nameof(Thread), new { post.Id });
        }

        
    }
}