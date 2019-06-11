using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Bazirano.Controllers
{
    public class NewsController : Controller
    {
        private INewsHelper helper;

        public NewsController(INewsHelper helper)
        {
            this.helper = helper;
        }

        public IActionResult Index()
        {
            return View(helper.CurrentNews);
        }
    }
}