using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Bazirano.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View(nameof(Index));
        }

        public IActionResult Article()
        {
            return View(nameof(Article));
        }

        public IActionResult Thread()
        {
            return View(nameof(Thread));
        }
    }
}
