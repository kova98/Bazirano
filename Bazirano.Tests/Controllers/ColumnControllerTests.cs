using Bazirano.Controllers;
using Bazirano.Infrastructure;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Bazirano.Models.Shared;
using Bazirano.Tests.Helpers;
using Bazirano.Tests.TestData;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bazirano.Tests.Controllers
{
    public class ColumnControllerTests
    {
        [Theory]
        [ClassData(typeof(ColumnTestData))]
        void Index_DisplaysViewWithCorrectModel(ColumnPost[] columnPosts, Author[] authors)
        {
            var columnRepoMock = new Mock<IColumnRepository>();
            columnRepoMock.Setup(x => x.ColumnPosts).Returns(columnPosts.AsQueryable());

            var columnController = new ColumnController(columnRepoMock.Object, null);
            
            var result = (ViewResult)columnController.Index();
            var model = (ColumnMainPageViewModel)result.Model;

            Assert.Equal(1, model.FirstColumn.Id);
            Assert.Equal(2, model.Columns.Count);
            Assert.Equal(2, model.Columns.First().Id);
            Assert.Equal(nameof(columnController.Index), result.ViewName);
        }

        [Fact]
        void Index_EmptyPosts_DisplaysViewWithCorrectModel()
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.ColumnPosts).Returns(new List<ColumnPost>().AsQueryable());
            var columnController = new ColumnController(mock.Object, null);

            var result = (ViewResult)columnController.Index();
            var model = (ColumnMainPageViewModel)result.Model;

            Assert.Equal(5, model.Columns.Count);
            Assert.Equal(nameof(columnController.Index), result.ViewName);
        }

        [Fact]
        void ColumnPost_ValidId_DisplaysViewWithCorrectModel()
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.ColumnPosts).Returns(new ColumnPost[] { new ColumnPost { Id = 1 } }.AsQueryable);
            var columnController = new ColumnController(mock.Object, null);

            var result = (ViewResult)columnController.ColumnPost(1);
            var model = (ColumnPost)result.Model;

            Assert.Equal(1, model.Id);
            Assert.NotNull(model.Comments);
            Assert.Equal(nameof(columnController.ColumnPost), result.ViewName);
        }

        [Fact]
        void ColumnPost_InvalidId_DisplaysErrorPage()
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.ColumnPosts).Returns(new ColumnPost[] { new ColumnPost { Id = 1 } }.AsQueryable);
            var columnController = new ColumnController(mock.Object, null);

            var result = (RedirectToActionResult)columnController.ColumnPost(2);

            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("ColumnPost", result.ActionName);
        }

        [Theory]
        [ClassData(typeof(ColumnTestData))]
        void Author_DisplaysViewWithCorrectModel(ColumnPost[] columnPosts, Author[] authors)
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.ColumnPosts).Returns(columnPosts.AsQueryable());
            mock.Setup(x => x.Authors).Returns(authors.AsQueryable());
            var columnController = new ColumnController(mock.Object, null);

            var result = (ViewResult)columnController.Author(1);
            var model = (AuthorPageViewModel)result.Model;

            Assert.Equal(1, model.Author.Id);
            Assert.Equal(2, model.Columns.Count);
            Assert.Equal(1, model.Columns[0].Id);
            Assert.Equal(2, model.Columns[1].Id);
            Assert.Equal(nameof(columnController.Author), result.ViewName);
        }

        [Theory]
        [InlineData("txt < 10", "username < 20")]
        [InlineData("text longer than 10", "username longer than 20")]
        async void PostComment_InvalidModel_DisplaysColumnPost(string commentText, string commentUsername)
        {
            var articlesRepoMock = new Mock<IColumnRepository>();
            articlesRepoMock.Setup(x => x.ColumnPosts).Returns(new ColumnPost[] { new ColumnPost { Id = 1 } }.AsQueryable);
            var columnController = new ColumnController(articlesRepoMock.Object, new RecaptchaMock());
            var viewModel = new ColumnRespondViewModel
            {
                ColumnId = 1,
                Comment = new Comment
                {
                    Text = commentText,
                    Username = commentUsername
                }
            };

            TestHelper.SimulateValidation(columnController, viewModel);
            TestHelper.SimulateValidation(columnController, viewModel.Comment);
            var result = (ViewResult)await columnController.PostComment(viewModel);

            Assert.Equal("ColumnPost", result.ViewName);
            articlesRepoMock.Verify(x => x.AddCommentToColumn(viewModel.Comment, viewModel.ColumnId), Times.Never);
        }

        [Fact]
        async void PostComment_ValidModel_RedirectsToColumnPost()
        {
            var articlesRepoMock = new Mock<IColumnRepository>();
            articlesRepoMock.Setup(x => x.ColumnPosts).Returns(new ColumnPost[] { new ColumnPost { Id = 1 } }.AsQueryable);
            var columnController = new ColumnController(articlesRepoMock.Object, new RecaptchaMock());
            var viewModel = new ColumnRespondViewModel { ColumnId = 1, Comment = new Comment { Text = "" } };

            TestHelper.SimulateValidation(columnController, viewModel);
            var result = (RedirectToActionResult)await columnController.PostComment(viewModel);

            Assert.Equal("ColumnPost", result.ActionName);
            Assert.Equal("Column", result.ControllerName);
        }

        [Fact]
        async void PostComment_ValidModel_CallsAddCommentToColumn()
        {
            var articlesRepoMock = new Mock<IColumnRepository>();
            articlesRepoMock.Setup(x => x.ColumnPosts).Returns(new ColumnPost[] { new ColumnPost { Id = 1 } }.AsQueryable);
            var columnController = new ColumnController(articlesRepoMock.Object, new RecaptchaMock());
            var viewModel = new ColumnRespondViewModel { ColumnId = 1, Comment = new Comment { Text = "" } };

            TestHelper.SimulateValidation(columnController, viewModel);
            var result = (RedirectToActionResult)await columnController.PostComment(viewModel);

            articlesRepoMock.Verify(x => x.AddCommentToColumn(viewModel.Comment, viewModel.ColumnId), Times.Once);
        }

        [Fact]
        async void PostComment_InvalidColumnId_DisplaysErrorView()
        {
            var columnRepoMock = new Mock<IColumnRepository>();
            columnRepoMock.Setup(x => x.ColumnPosts).Returns(new ColumnPost[] { new ColumnPost { Id = 1 } }.AsQueryable);
            var columnController = new ColumnController(columnRepoMock.Object, Mock.Of<IGoogleRecaptchaHelper>());
            var viewModel = new ColumnRespondViewModel { ColumnId = 2, Comment = new Comment { Text = "" } };

            TestHelper.SimulateValidation(columnController, viewModel);
            var result = (RedirectToActionResult)await columnController.PostComment(viewModel);

            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("Column", result.ActionName);
        }

        [Fact]
        async void PostComment_TrimsText()
        {
            var columnRepoMock = new Mock<IColumnRepository>();
            columnRepoMock.Setup(x => x.ColumnPosts).Returns(new ColumnPost[] { new ColumnPost { Id = 1 } }.AsQueryable);
            var columnController = new ColumnController(columnRepoMock.Object, new RecaptchaMock());
            var viewModel = new ColumnRespondViewModel { ColumnId = 1, Comment = new Comment { Text = "   test    " } };

            TestHelper.SimulateValidation(columnController, viewModel);
            var result = (RedirectToActionResult)await columnController.PostComment(viewModel);

            Assert.Equal("test", viewModel.Comment.Text);
        }
    }
}
