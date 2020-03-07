using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Bazirano.Controllers
{
    public class ErrorController : Controller
    {
        [Route("~/greska")]
        public IActionResult Index()
        {
            return View(nameof(Index));
        }

        [Route("~/greska-clanak")]
        public IActionResult Article()
        {
            return View(nameof(Article));
        }

        [Route("~/greska-dretva")]
        public IActionResult Thread()
        {
            return View(nameof(Thread));
        }
    }
}
