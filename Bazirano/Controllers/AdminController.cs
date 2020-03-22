using Microsoft.AspNetCore.Mvc;
using Bazirano.Models.DataAccess;
using System.Linq;
using Bazirano.Models.News;
using Microsoft.AspNetCore.Authorization;
using Bazirano.Infrastructure;
using System.Threading.Tasks;
using Bazirano.Models.Column;
using System;
using Bazirano.Models.Admin;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Bazirano.Models.AuthorInterface;

namespace Bazirano.Controllers
{
    [Authorize(Roles = "Admins")]
    public class AdminController : Controller
    {
        private NewsHelper newsHelper;
        private INewsPostsRepository newsRepo;
        private IBoardThreadsRepository boardRepo;
        private IColumnRepository columnRepo;
        private IColumnRequestsRepository columnRequestsRepo;
        private UserManager<IdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public AdminController(
            INewsPostsRepository newsRepo,
            IBoardThreadsRepository boardRepo,
            IColumnRepository columnRepo,
            IColumnRequestsRepository columnRequestsRepo,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.newsRepo = newsRepo;
            this.boardRepo = boardRepo;
            this.columnRepo = columnRepo;
            this.columnRequestsRepo = columnRequestsRepo;
            this.userManager = userManager;
            this.roleManager = roleManager;

            newsHelper = new NewsHelper(this.newsRepo);
        }

        public IActionResult Index()
        {
            return View(nameof(Index));
        }

        public IActionResult News()
        {
            List<NewsPost> newsPosts = newsRepo.NewsPosts.OrderByDescending(x => x.DatePosted).Take(100).ToList();

            return View(nameof(News), newsPosts);
        }

        public IActionResult DeleteArticle(long id)
        {   
            newsRepo.RemoveNewsPost(id);

            return RedirectToAction(nameof(News));
        }

        public IActionResult EditArticle(long id)
        {
            NewsPost post = newsRepo.NewsPosts.FirstOrDefault(x => x.Id == id);

            return View(nameof(EditArticle), post);
        }

        public IActionResult SaveArticle(NewsPost article)
        {
            newsRepo.EditNewsPost(article);

            return RedirectToAction(nameof(News));
        }

        public IActionResult DeleteBoardThread(long id)
        {
            boardRepo.RemoveThread(boardRepo.BoardThreads.FirstOrDefault(x => x.Id == id));

            return RedirectToAction(nameof(Board));
        }

        public IActionResult Board()
        {
            var boardThreads = boardRepo.BoardThreads.ToList().SortByBumpOrder();

            return View(nameof(Board), boardThreads);
        }
        
        public ViewResult Column()
        {
            var adminColumnViewModel = new AdminColumnViewModel
            {
                Authors = columnRepo.Authors
                    .OrderBy(a=>a.Name)
                    .ToList(),
                ColumnPosts = columnRepo.ColumnPosts
                    .OrderByDescending(p=>p.DatePosted)
                    .ToList(),
                ColumnRequests = columnRequestsRepo.ColumnRequests
                    .Where(r=>r.Status != ColumnRequestStatus.Draft)
                    .OrderBy(r=>r.DateRequested)
                    .ToList()
            };

            return View(nameof(Column), adminColumnViewModel);
        }

        public IActionResult AddColumn()
        {
            return View(nameof(AddColumn), new AddColumnViewModel
            {
                Column = new ColumnPost
                {
                    DatePosted = DateTime.Now,
                    Author = new Author()
                },
                Authors = columnRepo.Authors.ToList()
            });
        }

        public IActionResult SaveColumn(ColumnPost column)
        {
            columnRepo.SaveColumn(column);

            return Column();
        }

        public IActionResult DeleteColumn(long id)
        {
            columnRepo.DeleteColumn(id);

            return Column();
        }

        public IActionResult AddAuthor()
        {
            return View(nameof(AddAuthor), new Author());
        }

        public IActionResult SaveAuthor(Author author)
        {
            columnRepo.SaveAuthor(author);

            return Column();
        }

        public IActionResult DeleteAuthor(long id)
        {
            columnRepo.DeleteAuthor(id);

            return Column();
        }

        public IActionResult EditColumn(long id)
        {
            return View(nameof(AddColumn), new AddColumnViewModel
            {
                Column = columnRepo.ColumnPosts.First(p => p.Id == id),
                Authors = columnRepo.Authors.ToList()
            });
        }

        public IActionResult EditAuthor(long id)
        {
            return View(nameof(AddAuthor), columnRepo.Authors.First(a => a.Id == id));
        }

        public async Task<IActionResult> Accounts()
        {
            var users = new List<(IdentityUser, IList<string>)>();
            var accounts = userManager.Users.ToList();
            foreach(var account in accounts)
            {
                var roles = await userManager.GetRolesAsync(account);

                users.Add((account, roles));
            };

            var viewModel = new AccountsViewModel
            {
                UserRolesPairs = users,
                Roles = roleManager.Roles.ToList()
            };

            return View(nameof(Accounts), viewModel);
        }

        public async Task<IActionResult> CreateUser(string userName, string password)
        {
            var user = new IdentityUser(userName);
            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded == false)
            {
                ViewBag.Errors = result.Errors;
            }

            return await Accounts();
        }

        public async Task<IActionResult> DeleteUser(string name)
        {
            var user = await userManager.FindByNameAsync(name);

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded == false)
            {
                ViewBag.Errors = result.Errors;
            }

            return await Accounts();
        }

        public async Task<IActionResult> CreateRole(string roleName)
        {
            var role = new IdentityRole(roleName);
            var result = await roleManager.CreateAsync(role);

            if (result.Succeeded == false)
            {
                ViewBag.RoleErrors = result.Errors;
            }

            return await Accounts();
        }

        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            var result = await roleManager.DeleteAsync(role);

            if (result.Succeeded == false)
            {
                ViewBag.RoleErrors = result.Errors;
            }

            return await Accounts();
        }

        public async Task<IActionResult> AddUserToRole(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);

            var result = await userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded == false)
            {
                ViewBag.RoleErrors = result.Errors;
            }

            return await Accounts();
        }

        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);

            var result = await userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded == false)
            {
                ViewBag.RoleErrors = result.Errors;
            }

            return await Accounts();
        }

        public ViewResult ColumnRequest(long id)
        {
            var columnRequest = columnRequestsRepo.ColumnRequests.FirstOrDefault(r => r.Id == id);

            return View(nameof(ColumnRequest), columnRequest);
        } 

        public ViewResult UpdateColumnRequest(ColumnRequest columnRequest, string command)
        {
            columnRequest.Author = columnRepo.Authors.FirstOrDefault(a => a.Id == columnRequest.Author.Id);
            columnRequest.AdminApproved = User.Identity.Name;

            if (command == "revision")
            {
                return ReviseColumnRequest(columnRequest);
            }
            if (command == "publish")
            {
                return PublishColumnRequest(columnRequest);
            }
            if (command == "reject")
            {
                return RejectColumnRequest(columnRequest);
            }

            throw new ArgumentException($"Unknown command <{command}>", command);
        }

        private ViewResult RejectColumnRequest(ColumnRequest columnRequest)
        {
            columnRequest.Status = ColumnRequestStatus.Rejected;
            columnRequestsRepo.EditColumnRequest(columnRequest);

            return Column()
                .WithAlert(AlertType.Error, "Kolumna odbijena.");
        }

        private ViewResult PublishColumnRequest(ColumnRequest columnRequest)
        {
            columnRequest.Status = ColumnRequestStatus.Approved;
            columnRequestsRepo.EditColumnRequest(columnRequest);

            var columnPost = new ColumnPost
            {
                DatePosted = DateTime.Now,
                Author = columnRequest.Author,
                Title = columnRequest.ColumnTitle,
                Image = columnRequest.ColumnImage,
                Text = columnRequest.ColumnText
            };

            columnRepo.AddColumn(columnPost);

            return Column()
                .WithAlert(AlertType.Success, "Kolumna objavljena!");
        }

        private ViewResult ReviseColumnRequest(ColumnRequest columnRequest)
        {
            columnRequest.Status = ColumnRequestStatus.Revised;
            columnRequestsRepo.EditColumnRequest(columnRequest);

            return Column()
                .WithAlert(AlertType.Warning, "Kolumna spremljena kao revizija.");
        }
    }
}
