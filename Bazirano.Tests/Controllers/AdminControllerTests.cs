using Bazirano.Controllers;
using Bazirano.Models.Admin;
using Bazirano.Models.Board;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Bazirano.Tests.Controllers
{
    public class AdminControllerTests
    {
        [Fact]
        void Can_Display_Index()
        {
            // Arrange
            var mock = new Mock<INewsPostsRepository>();
            var adminController = new AdminController(mock.Object, null, null);

            // Act
            var result = adminController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(nameof(adminController.Index), viewResult.ViewName);
        }

        [Fact]
        void Can_Display_News_View_With_Latest_News()
        {
            // Arrange
            var mock = new Mock<INewsPostsRepository>();
            mock.Setup(x => x.NewsPosts).Returns(new NewsPost[]
            {
                new NewsPost { Id = 0, DatePosted = DateTime.Now.AddHours(-1) },
                new NewsPost { Id = 1, DatePosted = DateTime.Now.AddHours(-5) },
                new NewsPost { Id = 2, DatePosted = DateTime.Now.AddHours(5) },
                new NewsPost { Id = 3, DatePosted = DateTime.Now.AddHours(-2) },
                new NewsPost { Id = 4, DatePosted = DateTime.Now.AddHours(-10) }
            }
            .AsQueryable());

            var adminController = new AdminController(mock.Object, null, null);

            // Act
            var result = adminController.News();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var newsPosts = Assert.IsType<List<NewsPost>>(viewResult.Model);
            Assert.Equal(2, newsPosts.First().Id);
        }

        [Fact]
        void Can_Redirect_To_News_After_Deleting_Article()
        {
            // Arrange
            var mock = new Mock<INewsPostsRepository>();
            var adminController = new AdminController(mock.Object, null, null);

            // Act
            var result = adminController.DeleteArticle(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(adminController.News), redirectResult.ActionName);
        }

        [Fact]
        void Can_Display_EditArticle_With_Selected_Article()
        {
            // Arrange
            var mock = new Mock<INewsPostsRepository>();
            mock.Setup(x => x.NewsPosts).Returns(new NewsPost[]
            {
                new NewsPost { Id = 0 },
                new NewsPost { Id = 1 },
                new NewsPost { Id = 2 },
            }
            .AsQueryable());

            var adminController = new AdminController(mock.Object, null, null);

            // Act
            var result = adminController.EditArticle(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var newsPost = Assert.IsType<NewsPost>(viewResult.Model);
            Assert.Equal(nameof(adminController.EditArticle), viewResult.ViewName);
            Assert.Equal(1, newsPost.Id);
        }

        [Fact]
        void Can_Redirect_To_News_After_Saving_Article()
        {
            // Arrange
            var mock = new Mock<INewsPostsRepository>();
            var adminController = new AdminController(mock.Object, null, null);

            // Act
            var result = adminController.SaveArticle(new NewsPost());

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(adminController.News), redirectResult.ActionName);
        }

        [InlineData(1)]
        [Theory]
        void Can_Redirect_To_Board_After_Deleting_Board_Thread(long id)
        {
            // Arrange
            var mock = new Mock<IBoardThreadsRepository>();
            var adminController = new AdminController(null, mock.Object, null);

            // Act
            var result = adminController.DeleteBoardThread(id);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(adminController.Board), redirectResult.ActionName);
        }

        [Fact]
        void Can_Display_Board()
        {
            // Arrange
            var mock = new Mock<IBoardThreadsRepository>();
            var adminController = new AdminController(null, mock.Object, null);

            // Act
            var result = adminController.Board();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(nameof(adminController.Board), viewResult.ViewName);
            Assert.IsAssignableFrom<IEnumerable<BoardThread>>(viewResult.Model);
        }

        [Fact]
        void Can_Display_Column()
        {
            // Arrange
            var mock = new Mock<IColumnRepository>();
            var adminController = new AdminController(null, null, mock.Object);

            // Act
            var result = adminController.Column();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<AdminColumnViewModel>(viewResult.Model);
            Assert.IsType<List<Author>>(viewModel.Authors);
            Assert.IsType<List<ColumnPost>>(viewModel.ColumnPosts);
        }

        [Fact]
        void Can_Display_AddColumn()
        {
            // Arrange
            var mock = new Mock<IColumnRepository>();
            var adminController = new AdminController(null, null, mock.Object);

            // Act
            var result = adminController.AddColumn();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<AddColumnViewModel>(viewResult.Model);
            Assert.IsType<List<Author>>(viewModel.Authors);
            Assert.IsType<ColumnPost>(viewModel.Column);
        }

        [Fact]
        void Can_Display_Column_After_Saving_Column()
        {
            // Arrange
            var columnPost = new ColumnPost();
            var mock = new Mock<IColumnRepository>();
            var adminController = new AdminController(null, null, mock.Object);

            // Act
            var result = adminController.SaveColumn(columnPost);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(nameof(adminController.Column), viewResult.ViewName);
        }

        [Fact]
        void Can_Display_AddAuthor()
        {
            // Arrange
            var mock = new Mock<INewsPostsRepository>();
            var adminController = new AdminController(mock.Object, null, null);

            // Act
            var result = adminController.AddAuthor();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var author = Assert.IsType<Author>(viewResult.Model);

            Assert.Equal(nameof(adminController.AddAuthor), viewResult.ViewName);
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
        void Can_Display_Column_After_Saving_Author()
        {
            // Arrange
            var author = new Author();
            var mock = new Mock<IColumnRepository>();
            var adminController = new AdminController(null, null, mock.Object);

            // Act
            var result = adminController.SaveAuthor(author);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(nameof(adminController.Column), viewResult.ViewName);
        }

        [InlineData(1)]
        [Theory]
        void Can_Display_AddColumn_When_Calling_EditColumn(long id)
        {
            // Arrange
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.ColumnPosts).Returns(new ColumnPost[]
            {
                new ColumnPost { Id = 0 },
                new ColumnPost { Id = 1 },
                new ColumnPost { Id = 2 },
            }
            .AsQueryable());

            mock.Setup(x => x.Authors).Returns(new Author[]
            {
                new Author { Id = 0 },
                new Author { Id = 1 }
            }
            .AsQueryable());

            var adminController = new AdminController(null, null, mock.Object);

            // Act
            var result = adminController.EditColumn(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<AddColumnViewModel>(viewResult.Model);
            Assert.Equal(id, viewModel.Column.Id);
            Assert.Equal(2, viewModel.Authors.Count);
        }

        [InlineData(1)]
        [Theory]
        void Can_Display_AddAuthor_When_Calling_EditAuthor(long id)
        {
            // Arrange
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.Authors).Returns(new Author[]
            {
                new Author { Id = 0 },
                new Author { Id = 1 },
                new Author { Id = 2 }
            }
            .AsQueryable());

            var adminController = new AdminController(null, null, mock.Object);

            // Act
            var result = adminController.EditAuthor(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); 
            var author = Assert.IsType<Author>(viewResult.Model);
            Assert.Equal(nameof(adminController.AddAuthor), viewResult.ViewName);
            Assert.Equal(id, author.Id);
        }
    }
}
