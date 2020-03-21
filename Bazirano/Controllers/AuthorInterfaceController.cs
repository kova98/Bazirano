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
            var requests = GetAuthorRequests(User.Identity.Name);

            var viewModel = new AuthorInterfaceIndexViewModel
            {
                Author = columnRepository.Authors.FirstOrDefault(a => a.Name == User.Identity.Name),
                DraftRequests = requests.DraftRequests,
                PendingRequests = requests.PendingRequests,
                ApprovedRequests = requests.ApprovedRequests,
                RejectedRequests = requests.RejectedRequests
            };

            return View(nameof(Index), viewModel);
        }


        [Route("~/sucelje/moje-kolumne")]
        public ViewResult ColumnRequestsOverview()
        {
            var columnRequestsOverviewViewModel = GetAuthorRequests(User.Identity.Name);
            return View(nameof(ColumnRequestsOverview), columnRequestsOverviewViewModel);
        }

        public IActionResult SaveColumnRequest(ColumnRequest columnRequest, string command)
        {
            string message = "Kolumna uspješno spremljena kao skica.";
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
                columnRequest.Id = 0;
                columnRequestsRepository.AddColumnRequest(columnRequest);
            }

            return ColumnRequestsOverview()
                .WithAlert(AlertType.Success, message);
        }

        [Route("~/sucelje/obrada")]
        public IActionResult EditColumnRequest(long id)
        {
            var columnRequest
                = columnRequestsRepository.ColumnRequests
                    .FirstOrDefault(r => r.Id == id && r.Author.Name == User.Identity.Name)
                ?? GetPlaceholderColumnRequest();

            return View(nameof(EditColumnRequest), columnRequest);
        }

        [Route("~/sucelje/nova-kolumna")]
        public IActionResult NewColumnRequest()
        {
            var author = columnRepository.Authors.FirstOrDefault(a => a.Name == User.Identity.Name);

            if (author == null)
            {
                return RedirectToAction("NotAuthor", "Error");
            }

            return EditColumnRequest(0);
        }

        [Route("~/sucelje/izbrisi-skicu")]
        public IActionResult RemoveColumnRequest(long id)
        {
            var columnRequestExists = columnRequestsRepository.ColumnRequests
                    .Any(r => r.Id == id && r.Author.Name == User.Identity.Name);

            if (columnRequestExists)
            {
                columnRequestsRepository.RemoveColumnRequest(id);

                return ColumnRequestsOverview()
                    .WithAlert(AlertType.Success, "Skica uspješno izbrisana.");
            }

            return ColumnRequestsOverview()
                .WithAlert(AlertType.Error, "Greška: Skica ne postoji!");
        }

        private ColumnRequest GetPlaceholderColumnRequest()
        {
            var author = columnRepository.Authors.FirstOrDefault(a => a.Name == User.Identity.Name);

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

        private ColumnRequestsOverviewViewModel GetAuthorRequests(string authorName)
        {
            var requests = columnRequestsRepository.ColumnRequests.Where(c => c.Author.Name == authorName).ToList();

            var viewModel = new ColumnRequestsOverviewViewModel
            {
                DraftRequests = requests.Where(r => r.Status == ColumnRequestStatus.Draft).ToList(),
                PendingRequests = requests.Where(r => r.Status == ColumnRequestStatus.Pending).ToList(),
                ApprovedRequests = requests.Where(r => r.Status == ColumnRequestStatus.Approved).ToList(),
                RejectedRequests = requests.Where(r => r.Status == ColumnRequestStatus.Rejected).ToList()
            };

            return viewModel;
        }
    }
}
