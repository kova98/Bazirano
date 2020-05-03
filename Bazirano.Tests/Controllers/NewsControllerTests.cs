using Bazirano.Controllers;
using Bazirano.Infrastructure;
using Bazirano.Models.Admin;
using Bazirano.Models.Board;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Bazirano.Models.Home;
using Bazirano.Models.News;
using Bazirano.Models.Shared;
using Bazirano.Tests.Helpers;
using Bazirano.Tests.TestData;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bazirano.Tests.Controllers
{
    public class NewsControllerTests
    {
        [Fact]
        void Index_DisplaysViewWithCorrectModel()
        {
            var articlesRepoMock = new Mock<IArticleRepository>();
            var newsController = new NewsController(articlesRepoMock.Object, null);

            var result = (ViewResult)newsController.Index();

            Assert.Equal(nameof(newsController.Index), result.ViewName);
            Assert.IsType<NewsPageViewModel>(result.ViewData.Model);
        }

        [Fact]
        void Article_DisplaysViewWithCorrectModel()
        {
            var articlesRepoMock = new Mock<IArticleRepository>();
            articlesRepoMock.Setup(x => x.Articles).Returns(new Article[]
            {
                new Article { Id = 0}, new Article { Id = 1 }, new Article { Id = 2}
            }
            .AsQueryable());
            var newsController = new NewsController(articlesRepoMock.Object, null);

            var result = (ViewResult)newsController.Article(1);
            var viewModel = (ArticleViewModel)result.ViewData.Model;

            Assert.Equal(nameof(newsController.Article), result.ViewName);
            Assert.Equal(1, viewModel.Article.Id);
            articlesRepoMock.Verify(x => x.IncrementViewCount(viewModel.Article));
        }

        [Fact]
        void Article_InvalidArticleId_RedirectsToErrorView()
        {
            var articlesRepoMock = new Mock<IArticleRepository>();
            articlesRepoMock.Setup(x => x.Articles).Returns(new Article[]
            {
                new Article { Id = 1}
            }
            .AsQueryable());
            var newsController = new NewsController(articlesRepoMock.Object, null);

            var result = (RedirectToActionResult)newsController.Article(0);

            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("Article", result.ActionName);
        }

        [Fact]
        void PostNews_DistinctGuid_AddsArticle()
        {
            var articlesRepoMock = new Mock<IArticleRepository>();
            articlesRepoMock.Setup(x => x.Articles).Returns(new Article[] { new Article { Guid = 1 } }.AsQueryable());
            var newsController = new NewsController(articlesRepoMock.Object, null);
            var article = new Article { Guid = 2 };

            newsController.PostNews(article);

            articlesRepoMock.Verify(x => x.AddArticle(article));
        }

        [Fact]
        void PostNews_ExistingGuid_EditsPost()
        {
            var articlesRepoMock = new Mock<IArticleRepository>();
            articlesRepoMock.Setup(x => x.Articles).Returns(new Article[] { new Article { Guid = 1 } }.AsQueryable());
            var newsController = new NewsController(articlesRepoMock.Object, null);
            var article = new Article { Guid = 1 };

            newsController.PostNews(article);

            articlesRepoMock.Verify(x => x.EditArticle(article));
        }

        [Theory]
        [InlineData(null, 1, "a", "a", "a", "a")] // invalid title
        [InlineData("a", 0, "a", "a", "a", "a")]  // invalid guid
        [InlineData("a", 1, null, "a", "a", "a")] // invalid summary
        //[InlineData("a", 1, "a", null, "a", "a")] // invalid image
        [InlineData("a", 1, "a", "a", null, "a")] // invalid keywords
        [InlineData("a", 1, "a", "a", "a", null)] // invalid text
        void PostNews_InvalidModel_ReturnsBadRequest(string title, long guid, string summary, string image, string keywords, string text)
        {
            var articlesRepoMock = new Mock<IArticleRepository>();
            var newsController = new NewsController(articlesRepoMock.Object, null);
            var article = new Article
            {
                Title = title,
                Guid = guid,
                Summary = summary,
                Image = image,
                Keywords = keywords,
                Text = text
            };

            TestHelper.SimulateValidation(newsController, article);
            var result = newsController.PostNews(article);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData("a", 1, "a", "a", "a", "a")]
        void PostNews_ValidModel_ReturnsOk(string title, long guid, string summary, string image, string keywords, string text)
        {
            var articlesRepoMock = new Mock<IArticleRepository>();
            var newsController = new NewsController(articlesRepoMock.Object, null);
            var article = new Article
            {
                Title = title,
                Guid = guid,
                Summary = summary,
                Image = image,
                Keywords = keywords,
                Text = text
            };

            TestHelper.SimulateValidation(newsController, article);
            var result = newsController.PostNews(article);

            Assert.IsType<OkResult>(result);
        }
    }
}
