using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Bazirano.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult About()
        {
            return View(nameof(About));
        }

        public IActionResult Contact()
        {
            return View(nameof(Contact));
        }
    }
}