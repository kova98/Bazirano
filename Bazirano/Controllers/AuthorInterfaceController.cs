using Bazirano.Infrastructure;
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

        [Route("~/sucelje/moje-kolumne")]
        public ViewResult ColumnRequestsOverview()
        {
            var requests = columnRequestsRepository.ColumnRequests.Where(c => c.Author.Name == User.Identity.Name).ToList();

            var viewModel = new ColumnRequestsOverviewViewModel
            {
                DraftRequests = requests.Where(r => r.Status == ColumnRequestStatus.Draft).ToList(),
                PendingRequests = requests.Where(r => r.Status == ColumnRequestStatus.Pending).ToList(),
                ApprovedRequests = requests.Where(r => r.Status == ColumnRequestStatus.Approved).ToList(),
                RejectedRequests = requests.Where(r => r.Status == ColumnRequestStatus.Rejected).ToList()
            };

            return View(nameof(ColumnRequestsOverview), viewModel);
        }

        public IActionResult SaveColumnRequest(ColumnRequest columnRequest, string command)
        {
            string message = "Kolumna uspješno spremljena u skice.";
            columnRequest.DateRequested = DateTime.Now;

            if (command == "saveAndSend")
            {
                message = "Kolumna uspješno spremljena i poslana na pregled.";
                columnRequest.Status = ColumnRequestStatus.Pending;
            }

            var columnRequestExists = columnRequestsRepository.ColumnRequests.Any(c => c.Id == columnRequest.Id);
            if (columnRequestExists)
            {
                columnRequestsRepository.EditColumnRequest(columnRequest);
            }
            else
            {
                columnRequestsRepository.AddColumnRequest(columnRequest);
            }

            return ColumnRequestsOverview()
                .WithAlert(AlertType.Success, message);
        }

        [Route("~/sucelje/obrada")]
        public async Task<IActionResult> EditColumnRequest(long id)
        {
            var columnRequest
                = columnRequestsRepository.ColumnRequests.FirstOrDefault(r => r.Id == id)
                ?? await GetPlaceholderColumnRequest();

            return View(nameof(EditColumnRequest), columnRequest);
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

            return await EditColumnRequest(0);
        }

        private async Task<ColumnRequest> GetPlaceholderColumnRequest()
        {
            var user = await userManager.GetUserAsync(User);
            var author = columnRepository.Authors.FirstOrDefault(a => a.Name == user.UserName);

            var columnRequest = new ColumnRequest
            {
                Author = author,
                DateRequested = DateTime.Now,
                ColumnTitle = "Primjer naslova",
                ColumnImage = "https://i.imgur.com/DvQI0WC.png",
                ColumnText = System.IO.File.ReadAllText("sample-article.html")
            };

            return columnRequest;
        }
    }
}
