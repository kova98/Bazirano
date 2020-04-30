using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Models.Error;
using Microsoft.AspNetCore.Mvc;

namespace Bazirano.Controllers
{
    public class ErrorController : Controller
    {
        [Route("~/greska")]
        public IActionResult Index(ErrorViewModel viewModel = null)
        {
            return View("Index", new ErrorViewModel
            {
                RedirectActionName = viewModel.RedirectActionName ?? "Index",
                RedirectControllerName = viewModel.RedirectControllerName ?? "Home",
                Message = viewModel.Message ?? "Došlo je do pogreške",
                ButtonText = viewModel.ButtonText ?? "Povratak na naslovnicu"
            });
        }

        [Route("~/greska-clanak")]
        public IActionResult Article()
        {
            return Index(new ErrorViewModel
            {
                RedirectActionName = "Index",
                RedirectControllerName = "News",
                Message = "Taj članak ne postoji",
                ButtonText = "Povratak na vijesti"
            });
        }

        [Route("~/greska-dretva")]
        public IActionResult Thread()
        {
            return Index(new ErrorViewModel
            {
                RedirectActionName = "Catalog",
                RedirectControllerName = "Board",
                Message = "Ta dretva ne postoji",
                ButtonText = "Povratak na ploču"
            });
        }

        [Route("~/greska-nije-autor")]
        public IActionResult NotAuthor()
        {
            return Index(new ErrorViewModel
            {
                RedirectActionName = "Index",
                RedirectControllerName = "Home",
                Message = "Niste autor!",
                ButtonText = "Povratak na ploču"
            });
        }

        [Route("~/greska-pristup-odbijen")]
        public IActionResult AccessDenied()
        {
            return Index(new ErrorViewModel
            {
                Message = "Pristup odbijen!"
            });
        }
    }
}
