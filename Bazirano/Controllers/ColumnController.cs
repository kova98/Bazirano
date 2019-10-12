using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Bazirano.Controllers
{
    public class ColumnController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}