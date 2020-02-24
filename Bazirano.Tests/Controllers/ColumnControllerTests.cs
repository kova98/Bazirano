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
    public class ColumnControllerTests
    {
        [Theory]
        [ClassData(typeof(ColumnTestData))]
        void Index_DisplaysViewWithCorrectModel(ColumnPost[] columnPosts, Author[] authors)
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.ColumnPosts).Returns(columnPosts.AsQueryable());
            var columnController = new ColumnController(mock.Object);
            
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
            var columnController = new ColumnController(mock.Object);

            var result = (ViewResult)columnController.Index();
            var model = (ColumnMainPageViewModel)result.Model;

            Assert.Equal(5, model.Columns.Count);
            Assert.Equal(nameof(columnController.Index), result.ViewName);
        }

        [Theory]
        [ClassData(typeof(ColumnTestData))]
        void ColumnPost_DisplaysViewWithCorrectModel(ColumnPost[] columnPosts, Author[] authors)
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.ColumnPosts).Returns(columnPosts.AsQueryable());
            var columnController = new ColumnController(mock.Object);

            var result = (ViewResult)columnController.ColumnPost(1);
            var model = (ColumnPost)result.Model;

            Assert.Equal(1, model.Id);
            Assert.NotNull(model.Comments);
            Assert.Equal(nameof(columnController.ColumnPost), result.ViewName);
        }

        [Theory]
        [ClassData(typeof(ColumnTestData))]
        void Author_DisplaysViewWithCorrectModel(ColumnPost[] columnPosts, Author[] authors)
        {
            var mock = new Mock<IColumnRepository>();
            mock.Setup(x => x.ColumnPosts).Returns(columnPosts.AsQueryable());
            mock.Setup(x => x.Authors).Returns(authors.AsQueryable());
            var columnController = new ColumnController(mock.Object);

            var result = (ViewResult)columnController.Author(1);
            var model = (AuthorPageViewModel)result.Model;

            Assert.Equal(1, model.Author.Id);
            Assert.Equal(2, model.Columns.Count);
            Assert.Equal(1, model.Columns[0].Id);
            Assert.Equal(2, model.Columns[1].Id);
            Assert.Equal(nameof(columnController.Author), result.ViewName);
        }
    }
}
