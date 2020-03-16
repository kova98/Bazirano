using Bazirano.Models.AuthorInterface;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Controllers
{
    [Authorize(Roles = "Authors")]
    public class AuthorInterfaceController : Controller
    {
        private IColumnRequestsRepository columnRequestsRepository;
        private IColumnRepository columnRepository;
        private UserManager<IdentityUser> userManager;

        public AuthorInterfaceController(
            IColumnRequestsRepository columnRequestsRepository,
            IColumnRepository columnRepository,
            UserManager<IdentityUser> userManager)
        {
            this.columnRequestsRepository = columnRequestsRepository;
            this.columnRepository = columnRepository;
            this.userManager = userManager;
        }

        [Route("~/sucelje")]
        public IActionResult Index()
        {
            return View(nameof(Index));
        }

        [Route("~/sucelje/nova-kolumna")]
        public async Task<IActionResult> NewColumnRequest()
        {
            var user = await userManager.GetUserAsync(User);
            var author = columnRepository.Authors.FirstOrDefault(a => a.Name == user.UserName);

            if (author == null)
            {
                return RedirectToAction("NotAuthor", "Error");
            }

            var columnRequest = new ColumnRequest
            {
                Column = new ColumnPost
                {
                    Author = author,
                    DatePosted = DateTime.Now,
                    Title = "Primjer naslova",
                    Image = "https://i.imgur.com/DvQI0WC.png",
                    Text = ""
                }
            };

            return View(columnRequest);
        }

        public IActionResult AddColumnRequest(ColumnRequest columnRequest)
        {
            columnRequestsRepository.AddColumnRequest(columnRequest);

            return Index();
        }
    }
}
