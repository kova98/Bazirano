using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Bazirano.Controllers
{
    public class InfoController : Controller
    {
        [Route("~/o-nama")]
        public IActionResult About()
        {
            return View(nameof(About));
        }

        [Route("~/kontakt")]
        public IActionResult Contact()
        {
            return View(nameof(Contact));
        }
    }
}