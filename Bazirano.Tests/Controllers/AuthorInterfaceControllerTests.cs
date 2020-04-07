using Bazirano.Controllers;
using Bazirano.Infrastructure;
using Bazirano.Models.AuthorInterface;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Bazirano.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Bazirano.Tests.Controllers
{
    public class AuthorInterfaceControllerTests
    {
        [Fact]
        public void Index_UserIsNotAuthor_RedirectsToError()
        {
            var controller = GetMockAuthorInterfaceController();

            var result = (RedirectToActionResult)controller.Index();

            Assert.Equal("NotAuthor", result.ActionName);
            Assert.Equal("Error", result.ControllerName);
        }

        [Fact]
        public void Index_UserIsAuthor_DisplaysView()
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.Authors).Returns(new Author[] { new Author { Name = "TestUser" } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRepo: mock.Object);

            var result = (ViewResult)controller.Index();

            Assert.Equal("Index", result.ViewName);
        }

        [Fact]
        public void Index_ReturnsCorrectModel()
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.Authors).Returns(new Author[] { new Author { Name = "TestUser" } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRepo: mock.Object);

            var result = (ViewResult)controller.Index();
            var model = (AuthorInterfaceIndexViewModel)result.Model;

            Assert.Equal("TestUser", model.Author.Name);
            Assert.NotNull(model.DraftRequests);
            Assert.NotNull(model.PendingRequests);
            Assert.NotNull(model.ApprovedRequests);
            Assert.NotNull(model.RejectedRequests);
            Assert.NotNull(model.RevisedRequests);
        }

        [Fact]
        void ColumnRequestsOverview_ReturnsView()
        {
            var controller = GetMockAuthorInterfaceController();

            var result = controller.ColumnRequestsOverview();

            Assert.Equal("ColumnRequestsOverview", result.ViewName);
        }

        [Fact]
        void ColumnRequestsOverview_ReturnsCorrectModel()
        {
            var controller = GetMockAuthorInterfaceController();

            var result = controller.ColumnRequestsOverview();

            Assert.IsType<ColumnRequestsOverviewViewModel>(result.Model);
        }

        [Fact]
        void SaveColumnRequest_CallsColumnRequestsOverview()
        {
            var controller = GetMockAuthorInterfaceController();

            var result = controller.SaveColumnRequest(new ColumnRequest(), "");

            Assert.Equal("AuthorInterface", result.ControllerName);
            Assert.Equal("ColumnRequestsOverview", result.ActionName);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        void SaveColumnRequest_CallsEditColumnRequest(int id, int times)
        {
            var columnRequest = new ColumnRequest { Id = id};
            var mock = new Mock<IColumnRequestRepository>();
            mock.Setup(x => x.ColumnRequests).Returns(new ColumnRequest[] { new ColumnRequest { Id = 1 } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRequestsRepo: mock.Object);

            var result = controller.SaveColumnRequest(columnRequest, "");

            mock.Verify(x => x.EditColumnRequest(columnRequest), Times.Exactly(times));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        void SaveColumnRequest_CallsAddColumnRequest(int id, int times)
        {
            var columnRequest = new ColumnRequest { Id = id };
            var mock = new Mock<IColumnRequestRepository>();
            mock.Setup(x => x.ColumnRequests).Returns(new ColumnRequest[] { new ColumnRequest { Id = 1 } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRequestsRepo: mock.Object);

            var result = controller.SaveColumnRequest(columnRequest, "");

            mock.Verify(x => x.AddColumnRequest(columnRequest), Times.Exactly(times));
        }

        [Theory]
        [InlineData(1, "Test1")]
        [InlineData(2, "Test2")]
        [InlineData(3, "Test3")]
        void SaveColumnRequest_SetsAuthor(int id, string name)
        {
            var columnRequest = new ColumnRequest { Author = new Author { Id = id } };
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.Authors).Returns(new Author[] 
            { 
                new Author { Id = 1, Name = "Test1" },
                new Author { Id = 2, Name = "Test2" }, 
                new Author { Id = 3, Name = "Test3" } 
            }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRepo: mock.Object);

            var result = controller.SaveColumnRequest(columnRequest, "");

            Assert.Equal(name, columnRequest.Author.Name);
        }

        [Theory]
        [InlineData("saveAndSend", ColumnRequestStatus.Pending)]
        [InlineData("no command", ColumnRequestStatus.Draft)]
        void SaveColumnRequest_CommandAndRequestExists_SetsStatus(string command, ColumnRequestStatus status)
        {
            var columnRequest = new ColumnRequest { Id = 1, Status = ColumnRequestStatus.Draft };
            var mock = new Mock<IColumnRequestRepository>();
            mock.Setup(x => x.ColumnRequests).Returns(new ColumnRequest[] { new ColumnRequest { Id = 1 } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRequestsRepo: mock.Object);

            controller.SaveColumnRequest(columnRequest, command);

            Assert.Equal(status, columnRequest.Status);
        }

        [Theory]
        [InlineData(ColumnRequestStatus.Pending)]
        [InlineData(ColumnRequestStatus.Rejected)]
        [InlineData(ColumnRequestStatus.Approved)]
        [InlineData(ColumnRequestStatus.Revised)]
        void SaveColumnRequest_RequestDoesNotExistAndStatusNotDraft_ThrowsInvalidOperationException(ColumnRequestStatus status)
        {
            var columnRequest = new ColumnRequest { Status = status };
            var mock = new Mock<IColumnRequestRepository>();
            var controller = GetMockAuthorInterfaceController(columnRequestsRepo: mock.Object);

            Assert.Throws<InvalidOperationException>(() => controller.SaveColumnRequest(columnRequest, ""));
        }

        [Fact]
        void SaveColumnRequest_RequestDoesNotExistAndStatusDraft_CallsAddColumnRequest()
        {
            var columnRequest = new ColumnRequest { Status = ColumnRequestStatus.Draft };
            var mock = new Mock<IColumnRequestRepository>();
            var controller = GetMockAuthorInterfaceController(columnRequestsRepo: mock.Object);

            controller.SaveColumnRequest(columnRequest, "");

            mock.Verify(x => x.AddColumnRequest(columnRequest), Times.Once);
        }

        [Theory]
        [InlineData(ColumnRequestStatus.Draft, 1)]
        [InlineData(ColumnRequestStatus.Pending, 1)]
        [InlineData(ColumnRequestStatus.Rejected, 0)]
        [InlineData(ColumnRequestStatus.Approved, 0)]
        [InlineData(ColumnRequestStatus.Revised, 1)]
        void SaveColumnRequest_Status_CallsEditColumnRequest(ColumnRequestStatus status, int times)
        {
            var columnRequest = new ColumnRequest { Id = 1, Status = status };
            var mock = new Mock<IColumnRequestRepository>();
            mock.Setup(x => x.ColumnRequests).Returns(new ColumnRequest[] { new ColumnRequest { Id = 1 } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRequestsRepo: mock.Object);

            controller.SaveColumnRequest(columnRequest, "");

            mock.Verify(x => x.EditColumnRequest(columnRequest), Times.Exactly(times));
        }

        [Fact]
        public void EditColumnRequest_DisplaysView()
        {
            var controller = GetMockAuthorInterfaceController();

            var result = controller.EditColumnRequest(1);

            Assert.Equal("EditColumnRequest", result.ViewName);
        }

        [Fact]
        public void EditColumnRequest_ReturnsCorrectModel()
        {
            var controller = GetMockAuthorInterfaceController();

            var result = controller.EditColumnRequest(1);

            Assert.IsType<ColumnRequest>(result.Model);
        }

        [Fact]
        public void NewColumnRequest_AuthorDoesNotExist_RedirectsToError()
        {
            var controller = GetMockAuthorInterfaceController();

            var result = (RedirectToActionResult)controller.NewColumnRequest();

            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("NotAuthor", result.ActionName);
        }

        [Fact]
        public void NewColumnRequest_AuthorExists_DisplaysView()
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.Authors).Returns(new Author[] { new Author { Name = "TestUser" } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRepo: mock.Object);

            var result = (ViewResult)controller.NewColumnRequest();

            Assert.Equal("EditColumnRequest", result.ViewName);
        }

        [Fact]
        public void NewColumnRequest_AuthorExists_ReturnsCorrectModel()
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.Authors).Returns(new Author[] { new Author { Name = "TestUser" } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRepo: mock.Object);
            
            var result = (ViewResult)controller.NewColumnRequest();
            var model = (ColumnRequest)result.Model;

            Assert.Equal(0, model.Id);
        }

        [Theory]
        [InlineData(0,0)]
        [InlineData(1,1)]
        public void RemoveColumnRequest_AuthorId_CallsRemoveColumnRequest(int id, int times)
        {
            var columnRequestsRepoMock = new Mock<IColumnRequestRepository>();
            columnRequestsRepoMock
                .Setup(x => x.ColumnRequests)
                .Returns(new ColumnRequest[] { new ColumnRequest { Id = 1, Author = new Author { Name = "TestUser" } } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRequestsRepo: columnRequestsRepoMock.Object);

            var result = controller.RemoveColumnRequest(id);

            columnRequestsRepoMock.Verify(x => x.RemoveColumnRequest(id), Times.Exactly(times));
        }

        [Fact]
        public void RemoveColumnRequest_AuthorId_RedirectsToColumnRequestsOverview()
        {
            var columnRequestsRepoMock = new Mock<IColumnRequestRepository>();
            columnRequestsRepoMock
                .Setup(x => x.ColumnRequests)
                .Returns(new ColumnRequest[] { new ColumnRequest { Id = 1, Author = new Author { Name = "TestUser" } } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRequestsRepo: columnRequestsRepoMock.Object);

            var result = controller.RemoveColumnRequest(1);

            Assert.Equal("AuthorInterface", result.ControllerName);
            Assert.Equal("ColumnRequestsOverview", result.ActionName);
        }

        [Fact]
        public void EditProfile_ValidModel_RedirectsToIndex()
        {
            var author = new Author { Name = "test", Image = "test", Bio = "test", ShortBio = "test" };
            var controller = GetMockAuthorInterfaceController();

            TestHelper.SimulateValidation(controller, author);
            var result = (RedirectToActionResult)controller.EditProfile(author);

            Assert.Equal("Index", result.ActionName);
            Assert.Equal("AuthorInterface", result.ControllerName);
        }

        [Theory]
        [InlineData("", "test", "test", "test")] // invalid name
        [InlineData("test", "", "test", "test")] // invalid image
        [InlineData("test", "test", "", "test")] // invalid bio
        [InlineData("test", "test", "test", "")] // invalid shortBio
        public void EditProfile_InvalidModel_DisplaysIndex(string name, string image, string bio, string shortBio)
        {
            var author = new Author { Name = name, Image = image, Bio = bio, ShortBio = shortBio };
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.Authors).Returns(new Author[] { new Author { Name = "TestUser" } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRepo: mock.Object);

            TestHelper.SimulateValidation(controller, author);
            var result = (ViewResult)controller.EditProfile(author);

            Assert.Equal("Index", result.ViewName);
        }

        [Fact]
        public void EditProfile_InvalidModel_ReturnsModel()
        {
            var author = new Author();
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.Authors).Returns(new Author[] { new Author { Name = "TestUser" } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRepo: mock.Object);

            TestHelper.SimulateValidation(controller, author);
            var result = (ViewResult)controller.EditProfile(author);

            Assert.IsType<AuthorInterfaceIndexViewModel>(result.Model);
        }

        [Theory]
        [InlineData("", "test", "test")]      // currentPassword empty
        [InlineData(null, "test", "test")]    // currentPassword null
        [InlineData("test", "", "test")]      // newPassword empty
        [InlineData("test", null, "test")]    // newPassword null
        [InlineData("test", "test", "")]      // confirmPassword empty
        [InlineData("test", "test", null)]    // confirmPassword null
        [InlineData("test", "test", "test2")] // passwords don't match
        public async void ChangePassword_InvalidModel_DisplaysIndex(string currentPassword, string newPassword, string confirmPassword )
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.Authors).Returns(new Author[] { new Author { Name = "TestUser" } }.AsQueryable);
            var controller = GetMockAuthorInterfaceController(columnRepo: mock.Object);
            var model = new ChangePasswordViewModel(currentPassword, newPassword, confirmPassword);

            TestHelper.SimulateValidation(controller, model);
            var result = (ViewResult)await controller.ChangePassword(model);
            
            Assert.Equal("Index", result.ViewName);
        }

        #region Helper Methods

        private AuthorInterfaceController GetMockAuthorInterfaceController(
            IColumnRepository columnRepo = null,
            IColumnRequestRepository columnRequestsRepo = null)
        {
            columnRepo = columnRepo ?? Mock.Of<IColumnRepository>();
            columnRequestsRepo = columnRequestsRepo ?? Mock.Of<IColumnRequestRepository>();

            var userManager = MockHelper.GetMockUserManager();

            var writerMock = new Mock<IWriter>();
            writerMock.Setup(x => x.GetSampleColumnText()).Returns("");

            var controller = new AuthorInterfaceController(columnRequestsRepo, columnRepo, writerMock.Object, userManager);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "TestUser"),
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            controller.TempData = MockHelper.GetMockTempData();

            return controller;
        }

        #endregion
    }
}
