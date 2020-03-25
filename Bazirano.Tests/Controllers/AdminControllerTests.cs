using Bazirano.Controllers;
using Bazirano.Models.Admin;
using Bazirano.Models.AuthorInterface;
using Bazirano.Models.Board;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
using Bazirano.Tests.TestData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Bazirano.Tests.Controllers
{
    public class AdminControllerTests
    {
        [Fact]
        void Index_DisplaysView()
        {
            var adminController = GetMockAdminController();

            var result = (ViewResult)adminController.Index();

            Assert.Equal(nameof(adminController.Index), result.ViewName);
        }

        [Fact]
        void News_DisplaysView()
        {
            var adminController = GetMockAdminController();

            var result = (ViewResult)adminController.News();

            Assert.Equal(nameof(adminController.News), result.ViewName);
        }

        [Fact]
        void News_ReturnsCorrectModel()
        {
            var mock = new Mock<INewsPostsRepository>();
            mock.Setup(x => x.NewsPosts).Returns(new NewsPost[] 
            {
                new NewsPost { Id = 0, DatePosted = DateTime.Now.AddHours(-1) },
                new NewsPost { Id = 1, DatePosted = DateTime.Now.AddHours(0) },
                new NewsPost { Id = 2, DatePosted = DateTime.Now.AddHours(2) },
                new NewsPost { Id = 3, DatePosted = DateTime.Now.AddHours(1) },
            }
            .AsQueryable);
            var adminController = GetMockAdminController(newsRepo: mock);

            var result = (ViewResult)adminController.News();
            var newsPostsModel = (List<NewsPost>)result.Model;

            Assert.Equal(2, newsPostsModel.First().Id);
        }

        [Fact]
        void DeleteArticle_RedirectsToNews()
        {
            var adminController = GetMockAdminController();

            var result = (RedirectToActionResult)adminController.DeleteArticle(1);

            Assert.Equal(nameof(adminController.News), result.ActionName);
        }

        [Fact]
        void EditArticle_DisplaysView()
        {
            var adminController = GetMockAdminController();

            var result = (ViewResult)adminController.EditArticle(1);

            Assert.Equal(nameof(adminController.EditArticle), result.ViewName);
        }

        [Theory]
        [ClassData(typeof(NewsPostsTestData))]
        void EditArticle_ReturnsCorrectModel(NewsPost[] newsPosts)
        {
            var mock = new Mock<INewsPostsRepository>();
            mock.Setup(x => x.NewsPosts).Returns(newsPosts.AsQueryable());
            var adminController = new AdminController(mock.Object, null, null, null, null, null);

            var result = (ViewResult)adminController.EditArticle(1);

            var postModel = Assert.IsType<NewsPost>(result.Model);
            Assert.Equal(1, postModel.Id);
        }

        [Fact]
        void SaveArticle_RedirectsToNews()
        {
            var adminController = GetMockAdminController();

            var result = (RedirectToActionResult)adminController.SaveArticle(new NewsPost());

            Assert.Equal(nameof(adminController.News), result.ActionName);
        }

        [Fact]
        void DeleteBoardThread_RedirectsToBoard()
        {
            var adminController = GetMockAdminController();

            var result = (RedirectToActionResult)adminController.DeleteBoardThread(1);

            Assert.Equal(nameof(adminController.Board), result.ActionName);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(0, 0)]
        void DeleteBoardThread_CallsRemoveThread(long id, int times)
        {
            var mock = new Mock<IBoardThreadsRepository>();
            mock.Setup(x => x.BoardThreads).Returns(new BoardThread[] { new BoardThread { Id = 1 } }.AsQueryable);
            var adminController = GetMockAdminController(boardThreadsRepo: mock);

            var result = (RedirectToActionResult)adminController.DeleteBoardThread(1);

            mock.Verify(x => x.RemoveThread(id), Times.Exactly(times));
        }


        [Fact]
        void Board_DisplaysView()
        {
            var adminController = GetMockAdminController();

            var result = (ViewResult)adminController.Board();

            Assert.Equal(nameof(adminController.Board), result.ViewName);
        }

        [Theory]
        [ClassData(typeof(BoardThreadTestData))]
        void Board_ReturnsCorrectModel(BoardThread[] boardThreads)
        {
            var mock = new Mock<IBoardThreadsRepository>();
            mock.Setup(x => x.BoardThreads).Returns(boardThreads.AsQueryable());
            var adminController = new AdminController(null, mock.Object, null, null, null, null);

            var result = (ViewResult)adminController.Board();

            var firstThread = Assert.IsType<List<BoardThread>>(result.Model).First();
            Assert.Equal(1, firstThread.Id);
        }

        [Fact]
        void Column_DisplaysView()
        {
            var adminController = GetMockAdminController();

            var result = adminController.Column();

            Assert.Equal(nameof(adminController.Column), result.ViewName);
        }

        [Fact]
        void Column_ReturnsCorrectModel()
        {
            var adminController = GetMockAdminController();
            
            var result = adminController.Column();

            Assert.IsType<AdminColumnViewModel>(result.Model);
        }

        [Fact]
        void AddColumn_DisplaysView()
        {
            var adminController = GetMockAdminController();

            var result = (ViewResult)adminController.AddColumn();

            Assert.Equal(nameof(adminController.AddColumn), result.ViewName);
        }

        [Fact]
        void AddColumn_ReturnsCorrectModel()
        {
            var adminController = GetMockAdminController();

            var result = (ViewResult)adminController.AddColumn();

            Assert.IsType<AddColumnViewModel>(result.Model);
        }

        [Fact]
        void SaveColumn_DisplaysColumnView()
        {
            var adminController = GetMockAdminController();

            var result = (ViewResult)adminController.SaveColumn(new ColumnPost());

            Assert.Equal(nameof(adminController.Column), result.ViewName);
        }

        [Fact]
        void AddAuthor_DisplaysView()
        {
            var adminController = GetMockAdminController();

            var result = (ViewResult)adminController.AddAuthor();
            var author = (Author)result.Model;

            Assert.Equal(nameof(adminController.AddAuthor), result.ViewName);
            Assert.True(IsAuthorEmpty(author));
        }

        bool IsAuthorEmpty(Author author)
        {
            if (!(author.Id == 0)) return false;
            if (!string.IsNullOrEmpty(author.Bio)) return false;
            if (!string.IsNullOrEmpty(author.Image)) return false;
            if (!string.IsNullOrEmpty(author.Name)) return false;
            if (!string.IsNullOrEmpty(author.ShortBio)) return false;

            return true;
        }

        [Fact]
        void SaveAuthor_DisplaysColumnView()
        {
            var adminController = GetMockAdminController();

            var result = (ViewResult)adminController.SaveAuthor(new Author());

            Assert.Equal(nameof(adminController.Column), result.ViewName);
        }

        [Fact]
        void SaveAuthor_CallsSaveAuthor()
        {
            var author = new Author();
            var mock = new Mock<IColumnRepository>();
            var adminController = GetMockAdminController(columnRepo: mock);

            var result = (ViewResult)adminController.SaveAuthor(author);

            mock.Verify(x => x.SaveAuthor(author), Times.Once);
        }

        [Fact]
        void EditColumn_DisplaysAddColumn()
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.ColumnPosts).Returns(new ColumnPost[] { new ColumnPost { Id = 1 } }.AsQueryable);
            var adminController = GetMockAdminController(columnRepo: mock);

            var result = (ViewResult)adminController.EditColumn(1);

            Assert.Equal(nameof(adminController.AddColumn), result.ViewName);
        }

        [Theory]
        [ClassData(typeof(ColumnTestData))]
        void EditColumn_ReturnsCorrectModel(ColumnPost[] columnPosts, Author[] authors)
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.ColumnPosts).Returns(columnPosts.AsQueryable());
            mock.Setup(x => x.Authors).Returns(authors.AsQueryable());
            var adminController = GetMockAdminController(columnRepo: mock);

            var result = (ViewResult)adminController.EditColumn(1);

            var viewModel = Assert.IsType<AddColumnViewModel>(result.Model);
            Assert.Equal(1, viewModel.Column.Id);
            Assert.Equal(3, viewModel.Authors.Count);
        }

        [Fact]
        void EditAuthor_DisplaysAddAuthor()
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.Authors).Returns(new Author[] { new Author { Id = 1 } }.AsQueryable);
            var adminController = GetMockAdminController(columnRepo: mock);

            var result = (ViewResult)adminController.EditAuthor(1);

            Assert.Equal(nameof(adminController.AddAuthor), result.ViewName);
        }

        [Fact]
        void EditAuthor_ReturnsCorrectModel()
        {
            var authors = new List<Author> { new Author { Id = 0 }, new Author { Id = 1 }, new Author { Id = 2 } };
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.Authors).Returns(authors.AsQueryable());
            var adminController = GetMockAdminController(columnRepo: mock);

            var result = (ViewResult)adminController.EditAuthor(1);
            var author = (Author)result.Model;

            Assert.Equal(1, author.Id);
        }

        [Fact]
        async void Accounts_DisplaysView()
        {
            var adminController = GetMockAdminController();

            var result = (ViewResult)await adminController.Accounts();

            Assert.Equal(nameof(adminController.Accounts), result.ViewName);
        }

        [Fact]
        void ColumnRequest_DisplaysView()
        {
            var controller = GetMockAdminController();

            var result = controller.ColumnRequest(1);

            Assert.Equal(nameof(controller.ColumnRequest), result.ViewName);
        }

        [Fact]
        void ColumnRequest_ReturnsCorrectModel()
        {
            var mock = new Mock<IColumnRequestsRepository>();
            mock.Setup(x => x.ColumnRequests).Returns(new ColumnRequest[] 
            { 
                new ColumnRequest { Id = 0 },
                new ColumnRequest { Id = 1 },
                new ColumnRequest { Id = 2 } 
            }
            .AsQueryable);
            var controller = GetMockAdminController(columnRequestsRepo: mock);

            var result = controller.ColumnRequest(1);

            var model = (ColumnRequest)result.Model;
            Assert.Equal(1, model.Id);
        }

        [Theory]
        [InlineData("revise")]
        [InlineData("publish")]
        [InlineData("reject")]
        void UpdateColumnRequest_ValidCommand_CallsEditColumnRequest(string command)
        {
            var request = new ColumnRequest();
            var mock = new Mock<IColumnRequestsRepository>();
            var controller = GetMockAdminController(columnRequestsRepo: mock);

            var result = controller.UpdateColumnRequest(request, command);

            mock.Verify(x => x.EditColumnRequest(request), Times.Once);
        }

        [Fact]
        void UpdateColumnRequest_InvalidCommand_ThrowsException()
        {
            var request = new ColumnRequest();
            var mock = new Mock<IColumnRequestsRepository>();
            var controller = GetMockAdminController(columnRequestsRepo: mock);

            Assert.Throws<ArgumentException>(() => controller.UpdateColumnRequest(request, "test"));
        }

        [Theory]
        [InlineData("revise", ColumnRequestStatus.Revised)]
        [InlineData("publish", ColumnRequestStatus.Approved)]
        [InlineData("reject", ColumnRequestStatus.Rejected)]
        void UpdateColumnRequest_ValidCommand_SetsStatus(string command, ColumnRequestStatus status)
        {
            var request = new ColumnRequest();
            var mock = new Mock<IColumnRequestsRepository>();
            var controller = GetMockAdminController(columnRequestsRepo: mock);

            controller.UpdateColumnRequest(request, command);

            Assert.Equal(status, request.Status);
        }

        [Fact]
        void UpdateColumnRequest_PublishCommand_SetsDateApproved()
        {
            var request = new ColumnRequest();
            var mock = new Mock<IColumnRequestsRepository>();
            var controller = GetMockAdminController(columnRequestsRepo: mock);

            controller.UpdateColumnRequest(request, "publish");

            Assert.NotEqual(DateTime.MinValue, request.DateApproved);
        }

        [Fact]
        void UpdateColumnRequest_PublishCommand_CallsAddColumnRequest()
        {
            var request = new ColumnRequest();
            var mock = new Mock<IColumnRepository>();
            var controller = GetMockAdminController(columnRepo: mock);

            controller.UpdateColumnRequest(request, "publish");

            mock.Verify(x => x.AddColumn(
                It.Is<ColumnPost>(p =>
                    (p.DatePosted == request.DateApproved) &&
                    (p.Author == request.Author) &&
                    (p.Title == request.ColumnTitle) &&
                    (p.Image == request.ColumnImage) &&
                    (p.Text == request.ColumnText))));
        }

        #region Helper Methods

        private AdminController GetMockAdminController(
            Mock<INewsPostsRepository> newsRepo = null,
            Mock<IBoardThreadsRepository> boardThreadsRepo = null,
            Mock<IColumnRepository> columnRepo = null,
            Mock<IColumnRequestsRepository> columnRequestsRepo = null)
        {
            var controller = new AdminController
            (
                newsRepo != null ? newsRepo.Object : Mock.Of<INewsPostsRepository>(),
                boardThreadsRepo != null ? boardThreadsRepo.Object : Mock.Of<IBoardThreadsRepository>(),
                columnRepo != null ? columnRepo.Object : Mock.Of<IColumnRepository>(),
                columnRequestsRepo != null ? columnRequestsRepo.Object : Mock.Of<IColumnRequestsRepository>(),
                MockHelper.GetMockUserManager(),
                MockHelper.GetMockRoleManager()
            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "Test")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            controller.TempData = MockHelper.GetMockTempData();

            return controller;
        }

        private AdminController GetMockAdminController()
        {
            return new AdminController
            (
                new Mock<INewsPostsRepository>().Object,
                new Mock<IBoardThreadsRepository>().Object,
                new Mock<IColumnRepository>().Object,
                new Mock<IColumnRequestsRepository>().Object,
                MockHelper.GetMockUserManager(),
                MockHelper.GetMockRoleManager()
            );
        }

        #endregion
    }
}
