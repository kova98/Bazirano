﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Models;
using Bazirano.Models.DataAccess;
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

        public IActionResult Thread(int id)
        {
            return View(repository.BoardThreads
                    .FirstOrDefault(t => t.Posts.First().Id == id));
        }

        [HttpPost]
        public RedirectToActionResult CreateThread(BoardPost post)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Submit));
            }   

            post.DatePosted = DateTime.Now;

            BoardThread thread = new BoardThread
            {
                PostCount = 0,
                ImageCount = string.IsNullOrEmpty(post.Image) ? 0 : 1,
                IsLocked = false,
                Posts = new List<BoardPost> { post }
            };

            repository.AddThread(thread);

            return RedirectToAction(nameof(Thread), thread);
        }
    }
}