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
        private IColumnRequestRepository columnRequestsRepository;
        private IColumnRepository columnRepository;
        private IWriter writerHelper;
        private UserManager<IdentityUser> userManager;

        public AuthorInterfaceController(
            IColumnRequestRepository columnRequestsRepository,
            IColumnRepository columnRepository,
            IWriter writerHelper,
            UserManager<IdentityUser> userManager)
        {
            this.columnRequestsRepository = columnRequestsRepository;
            this.columnRepository = columnRepository;
            this.writerHelper = writerHelper;
            this.userManager = userManager;
        }

        [Route("~/sucelje")]
        public IActionResult Index()
        {
            var author = columnRepository.Authors.FirstOrDefault(a => a.Name == User.Identity.Name);

            if (author == null)
            {
                return RedirectToAction("NotAuthor", "Error");
            }

            var viewModel = GetAuthorRequests(author);

            return View(nameof(Index), viewModel);
        }

        [Route("~/postavke")]
        public IActionResult Settings()
        {
            var author = columnRepository.Authors.FirstOrDefault(a => a.Name == User.Identity.Name);

            return View("Settings", author);
        }

        public RedirectToActionResult SaveColumnRequest(ColumnRequest columnRequest, string command)
        {
            columnRequest.Author = columnRepository.Authors.FirstOrDefault(a => a.Id == columnRequest.Author.Id);

            var columnRequestExists = columnRequestsRepository.ColumnRequests.Any(c => c.Id == columnRequest.Id);
            if (columnRequestExists)
            {
                EditExistingColumnRequest(columnRequest, command);
            }
            else
            {
                AddNewColumnRequest(columnRequest, command);
            }

            return RedirectToAction("Index" , "AuthorInterface");
        }

        private void AddNewColumnRequest(ColumnRequest columnRequest, string command)
        {
            if (columnRequest.Status == ColumnRequestStatus.Draft)
            {
                if (command == "saveAndSend")
                {
                    Alert.Add(this, AlertType.Success, "Kolumna uspješno spremljena i poslana na pregled.");
                    columnRequest.Status = ColumnRequestStatus.Pending;
                    columnRequest.DateRequested = DateTime.Now;
                }
                else
                {
                    Alert.Add(this, AlertType.Success, "Kolumna uspješno spremljena kao skica.");
                }

                columnRequest.Id = 0;
                columnRequestsRepository.AddColumnRequest(columnRequest);
            }
            else
            {
                throw new InvalidOperationException("Tried to Add a new ColumnRequest with a non-default (Draft) Status");
            }
        }

        private void EditExistingColumnRequest(ColumnRequest columnRequest, string command)
        {
            if (columnRequest.Status != ColumnRequestStatus.Rejected &&
                columnRequest.Status != ColumnRequestStatus.Approved)
            {
                if (command == "saveAndSend")
                {
                    Alert.Add(this, AlertType.Success, "Kolumna uspješno spremljena i poslana na pregled.");
                    columnRequest.Status = ColumnRequestStatus.Pending;
                    columnRequest.DateRequested = DateTime.Now;
                }

                columnRequestsRepository.EditColumnRequest(columnRequest);
            }
            else
            {
                Alert.Add(this, AlertType.Error,
                    "Greška",
                    "Mogu se uređivati samo skice ");
            }
        }

        [Route("~/sucelje/obrada")]
        public ViewResult EditColumnRequest(long id)
        {
            var columnRequest
                = columnRequestsRepository.ColumnRequests
                    .FirstOrDefault(r => r.Id == id && r.Author.Name == User.Identity.Name)
                ?? GetPlaceholderColumnRequest();

            if (columnRequest.Status == ColumnRequestStatus.Revised)
            {
                Alert.Add(this, 
                    AlertType.Warning,
                    "Revizija",
                    "Administrator je pregledao vašu kolumnu i predložio izmjene.",
                    columnRequest.AdminRemarks);
            }

            if (columnRequest.Status == ColumnRequestStatus.Rejected)
            {
                Alert.Add(this, 
                    AlertType.Error,
                    "Odbijeno",
                    "Administrator je odbio vašu kolumnu.",
                    columnRequest.AdminRemarks);
            }

            if (columnRequest.Status == ColumnRequestStatus.Approved)
            {     
                Alert.Add(this,
                    AlertType.Success,
                    "Odobreno",
                    "Administrator je odobrio i objavio vašu kolumnu!");
            }

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
        public RedirectToActionResult RemoveColumnRequest(long id)
        {
            var columnRequestExists = columnRequestsRepository.ColumnRequests
                .Any(r => r.Id == id && r.Author.Name == User.Identity.Name);

            if (columnRequestExists)
            {
                columnRequestsRepository.RemoveColumnRequest(id);

                Alert.Add(this, AlertType.Success, "Skica uspješno izbrisana.");
            }
            else
            {
                Alert.Add(this, AlertType.Error, "Greška: Skica ne postoji!");
            }

            return RedirectToAction("Index", "AuthorInterface");
        }

        public IActionResult EditProfile(Author author)
        {
            if (ModelState.IsValid)
            {
                columnRepository.SaveAuthor(author);
                Alert.Add(this, AlertType.Success, "Profil uspješno izmijenjen!");
                return RedirectToAction("Index", "AuthorInterface");
            }
            else
            {
                Alert.Add(this, AlertType.Error,
                    "Greška",
                    "Došlo je do pogreške. Molimo provjerite upisane podatke");
                return Index();
            }
        }

        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                Alert.Add(this, AlertType.Error, "Došlo je do pogreške. Molimo, provjerite upisane podatke.");
                return Index();
            }

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                Alert.Add(this, AlertType.Success, "Lozinka uspješno promijenjena!");
            }
            else
            {
                Alert.Add(this, AlertType.Error, "Došlo je do pogreške. Molimo, provjerite upisane podatke.");
            }

            return RedirectToAction("Index", "AuthorInterface");
        }

        public PartialViewResult GetColumnPostPreview(ColumnPost columnPost)
        {
            return PartialView("ColumnPostBody", columnPost);
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
                ColumnText = writerHelper.GetSampleColumnText()
            };

            return columnRequest;
        }

        private AuthorInterfaceViewModel GetAuthorRequests(Author author)
        {
            var requests = columnRequestsRepository.ColumnRequests.Where(c => c.Author.Name == author.Name).ToList();

            var viewModel = new AuthorInterfaceViewModel
            {
                Author = author,
                DraftRequests = requests.Where(r => r.Status == ColumnRequestStatus.Draft).ToList(),
                PendingRequests = requests.Where(r => r.Status == ColumnRequestStatus.Pending).ToList(),
                ApprovedRequests = requests.Where(r => r.Status == ColumnRequestStatus.Approved).ToList(),
                RejectedRequests = requests.Where(r => r.Status == ColumnRequestStatus.Rejected).ToList(),
                RevisedRequests = requests.Where(r => r.Status == ColumnRequestStatus.Revised).ToList()
            };

            return viewModel;
        }
    }
}
