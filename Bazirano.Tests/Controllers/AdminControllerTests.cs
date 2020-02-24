using Bazirano.Controllers;
using Bazirano.Models.Admin;
using Bazirano.Models.Board;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
using Bazirano.Tests.TestData;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bazirano.Tests.Controllers
{
    public class AdminControllerTests
    {
        [Fact]
        void Index_DisplaysView()
        {
            var mock = new Mock<INewsPostsRepository>();
            var adminController = new AdminController(mock.Object, null, null);

            var result = (ViewResult)adminController.Index(); 

            Assert.Equal(nameof(adminController.Index), result.ViewName);
        }

        [Theory]
        [ClassData(typeof(NewsPostsTestData))]
        void News_DisplaysViewWithCorrectModel(NewsPost[] newsPosts)
        {
            var mock = new Mock<INewsPostsRepository>(); 
            mock.Setup(x => x.NewsPosts).Returns(newsPosts.AsQueryable());
            var adminController = new AdminController(mock.Object, null, null);

            var result = (ViewResult)adminController.News();
            var newsPostsModel = (List<NewsPost>)result.Model;

            Assert.Equal(2, newsPostsModel.First().Id);
            Assert.Equal(nameof(adminController.News), result.ViewName);
        }

        [Fact]
        void DeleteArticle_RedirectsToNews()
        {
            var mock = new Mock<INewsPostsRepository>();
            var adminController = new AdminController(mock.Object, null, null);

            var result = (RedirectToActionResult)adminController.DeleteArticle(1);

            Assert.Equal(nameof(adminController.News), result.ActionName);
        }

        [Theory]
        [ClassData(typeof(NewsPostsTestData))]
        void EditArticle_DisplaysViewWithCorrectModel(NewsPost[] newsPosts)
        {
            var mock = new Mock<INewsPostsRepository>();
            mock.Setup(x => x.NewsPosts).Returns(newsPosts.AsQueryable());
            var adminController = new AdminController(mock.Object, null, null);

            var result = (ViewResult)adminController.EditArticle(1);
            var postModel = (NewsPost)result.Model;

            Assert.Equal(nameof(adminController.EditArticle), result.ViewName);
            Assert.Equal(1, postModel.Id);
        }

        [Fact]
        void SaveArticle_RedirectsToNews()
        {
            var mock = new Mock<INewsPostsRepository>();
            var adminController = new AdminController(mock.Object, null, null);

            var result = (RedirectToActionResult)adminController.SaveArticle(new NewsPost());

            Assert.Equal(nameof(adminController.News), result.ActionName);
        }

        [InlineData(1)]
        [Theory]
        void DeleteBoardThread_RedirectsToBoard(long id)
        {
            var mock = new Mock<IBoardThreadsRepository>();
            var adminController = new AdminController(null, mock.Object, null);

            var result = (RedirectToActionResult)adminController.DeleteBoardThread(id);

            Assert.Equal(nameof(adminController.Board), result.ActionName);
        }

        [Theory]
        [ClassData(typeof(BoardThreadTestData))]
        void Board_DisplaysViewWithCorrectModel(BoardThread[] boardThreads)
        {
            var mock = new Mock<IBoardThreadsRepository>();
            mock.Setup(x => x.BoardThreads).Returns(boardThreads.AsQueryable());
            var adminController = new AdminController(null, mock.Object, null);

            var result = (ViewResult)adminController.Board();
            var firstThread = (result.Model as List<BoardThread>).First();

            Assert.Equal(nameof(adminController.Board), result.ViewName);
            Assert.Equal(1, firstThread.Id);
        }

        [Fact]
        void Column_DisplaysView()
        {
            var mock = new Mock<IColumnRepository>();
            var adminController = new AdminController(null, null, mock.Object);

            var result = (ViewResult)adminController.Column();
            var viewModel = (AdminColumnViewModel)result.Model;

            Assert.Equal(nameof(adminController.Column), result.ViewName);
            Assert.IsType<List<Author>>(viewModel.Authors);
            Assert.IsType<List<ColumnPost>>(viewModel.ColumnPosts);
        }

        [Fact]
        void AddColumn_DisplaysView()
        {
            var mock = new Mock<IColumnRepository>();
            var adminController = new AdminController(null, null, mock.Object);

            var result = (ViewResult)adminController.AddColumn();
            var viewModel = (AddColumnViewModel)result.Model;

            Assert.Equal(nameof(adminController.AddColumn), result.ViewName);
            Assert.IsType<List<Author>>(viewModel.Authors);
            Assert.IsType<ColumnPost>(viewModel.Column);
        }

        [Fact]
        void SaveColumn_DisplaysColumnView()
        {
            var mock = new Mock<IColumnRepository>();
            var adminController = new AdminController(null, null, mock.Object);
            var columnPost = new ColumnPost();

            var result = (ViewResult)adminController.SaveColumn(columnPost);

            Assert.Equal(nameof(adminController.Column), result.ViewName);
        }

        [Fact]
        void AddAuthor_DisplaysView()
        {
            var mock = new Mock<INewsPostsRepository>();
            var adminController = new AdminController(mock.Object, null, null);

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
            var mock = new Mock<IColumnRepository>();
            var adminController = new AdminController(null, null, mock.Object); 
            var author = new Author();

            var result = (ViewResult)adminController.SaveAuthor(author);

            Assert.Equal(nameof(adminController.Column), result.ViewName);
        }

        [Theory]
        [ClassData(typeof(ColumnTestData))]
        void EditColumn_DisplaysAddColumn(ColumnPost[] columnPosts, Author[] authors)
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.ColumnPosts).Returns(columnPosts.AsQueryable());
            mock.Setup(x => x.Authors).Returns(authors.AsQueryable());
            var adminController = new AdminController(null, null, mock.Object);

            var result = (ViewResult)adminController.EditColumn(1);
            var viewModel = (AddColumnViewModel)result.Model;

            Assert.Equal(nameof(adminController.AddColumn), result.ViewName);
            Assert.Equal(1, viewModel.Column.Id);
            Assert.Equal(3, viewModel.Authors.Count);
        }

        [Fact]
        void EditAuthor_DisplaysAddAuthorWithCorrectModel()
        {
            var authors = new List<Author> { new Author { Id = 0 }, new Author { Id = 1 }, new Author { Id = 2 } };
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.Authors).Returns(authors.AsQueryable());
            var adminController = new AdminController(null, null, mock.Object);

            var result = (ViewResult)adminController.EditAuthor(1);
            var author = (Author)result.Model;

            Assert.Equal(nameof(adminController.AddAuthor), result.ViewName);
            Assert.Equal(1, author.Id);
        }
    }
}
