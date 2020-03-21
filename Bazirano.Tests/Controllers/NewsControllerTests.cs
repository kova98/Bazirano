using Bazirano.Controllers;
using Bazirano.Models.Admin;
using Bazirano.Models.Board;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Bazirano.Models.Home;
using Bazirano.Models.News;
using Bazirano.Models.Shared;
using Bazirano.Tests.Infrastructure;
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
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            var newsController = new NewsController(newsPostsRepoMock.Object, null);

            var result = (ViewResult)newsController.Index();

            Assert.Equal(nameof(newsController.Index), result.ViewName);
            Assert.IsType<NewsPageViewModel>(result.ViewData.Model);
        }

        [Fact]
        void Article_DisplaysViewWithCorrectModel()
        {
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            newsPostsRepoMock.Setup(x => x.NewsPosts).Returns(new NewsPost[]
            {
                new NewsPost { Id = 0}, new NewsPost { Id = 1 }, new NewsPost { Id = 2}
            }
            .AsQueryable());
            var newsController = new NewsController(newsPostsRepoMock.Object, null);

            var result = (ViewResult)newsController.Article(1);
            var viewModel = (ArticleViewModel)result.ViewData.Model;

            Assert.Equal(nameof(newsController.Article), result.ViewName);
            Assert.Equal(1, viewModel.Article.Id);
            newsPostsRepoMock.Verify(x => x.IncrementViewCount(viewModel.Article));
        }

        [Fact]
        void Article_InvalidArticleId_RedirectsToErrorView()
        {
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            newsPostsRepoMock.Setup(x => x.NewsPosts).Returns(new NewsPost[]
            {
                new NewsPost { Id = 1}
            }
            .AsQueryable());
            var newsController = new NewsController(newsPostsRepoMock.Object, null);

            var result = (RedirectToActionResult)newsController.Article(0);

            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("Article", result.ActionName);
        }

        [Theory]
        [InlineData("text longer than 10", "username < 20", 1)]
        [InlineData("txt < 10", "username < 20", 0)]
        [InlineData("text longer than 10", "username longer than 20", 0)]
        async void PostComment_ValidModel_DisplaysArticleViewAndAddsComment(string commentText, string commentUsername, int timesCalled)
        {
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            newsPostsRepoMock.Setup(x => x.NewsPosts).Returns(new NewsPost[] { new NewsPost { Id = 1 } }.AsQueryable);
            var newsController = new NewsController(newsPostsRepoMock.Object, new RecaptchaMock());
            var viewModel = new ArticleRespondViewModel
            {
                ArticleId = 1,
                Comment = new Comment
                {
                    Text = commentText,
                    Username = commentUsername
                }
            };

            TestHelper.SimulateValidation(newsController, viewModel);
            TestHelper.SimulateValidation(newsController, viewModel.Comment);
            var result = (ViewResult)await newsController.PostComment(viewModel);

            Assert.Equal(nameof(newsController.Article), result.ViewName);
            newsPostsRepoMock.Verify(x => x.AddCommentToNewsPost(viewModel.Comment, viewModel.ArticleId), Times.Exactly(timesCalled));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        async void PostComment_InvalidArticleId_DisplaysErrorView(long articleId)
        {
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            newsPostsRepoMock.Setup(x => x.NewsPosts).Returns(new NewsPost[] { new NewsPost { Id = 1 } }.AsQueryable);
            var newsController = new NewsController(newsPostsRepoMock.Object, null);
            var viewModel = new ArticleRespondViewModel { ArticleId = articleId };

            TestHelper.SimulateValidation(newsController, viewModel);
            var result = (RedirectToActionResult)await newsController.PostComment(viewModel);

            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("Article", result.ActionName);
        }

        [Fact]
        async void PostComment_TrimsText()
        {
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            newsPostsRepoMock.Setup(x => x.NewsPosts).Returns(new NewsPost[] { new NewsPost { Id = 1 } }.AsQueryable);
            var newsController = new NewsController(newsPostsRepoMock.Object, new RecaptchaMock());
            var viewModel = new ArticleRespondViewModel { ArticleId = 1, Comment = new Comment { Text = "   test    " } };

            TestHelper.SimulateValidation(newsController, viewModel);
            var result = (ViewResult)await newsController.PostComment(viewModel);

            Assert.Equal("test", viewModel.Comment.Text);
        }

        [Fact]
        void PostNews_DistinctGuid_AddsNewsPost()
        {
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            newsPostsRepoMock.Setup(x => x.NewsPosts).Returns(new NewsPost[] { new NewsPost { Guid = 1 } }.AsQueryable());
            var newsController = new NewsController(newsPostsRepoMock.Object, null);
            var newsPost = new NewsPost { Guid = 2 };

            newsController.PostNews(newsPost);

            newsPostsRepoMock.Verify(x => x.AddNewsPost(newsPost));
        }

        [Fact]
        void PostNews_ExistingGuid_EditsPost()
        {
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            newsPostsRepoMock.Setup(x => x.NewsPosts).Returns(new NewsPost[] { new NewsPost { Guid = 1 } }.AsQueryable());
            var newsController = new NewsController(newsPostsRepoMock.Object, null);
            var newsPost = new NewsPost { Guid = 1 };

            newsController.PostNews(newsPost);

            newsPostsRepoMock.Verify(x => x.EditNewsPost(newsPost));
        }

        [Theory]
        [InlineData(null, 1, "a", "a", "a", "a")] // invalid title
        [InlineData("a", 0, "a", "a", "a", "a")]  // invalid guid
        [InlineData("a", 1, null, "a", "a", "a")] // invalid summary
        [InlineData("a", 1, "a", null, "a", "a")] // invalid image
        [InlineData("a", 1, "a", "a", null, "a")] // invalid keywords
        [InlineData("a", 1, "a", "a", "a", null)] // invalid text
        void PostNews_InvalidModel_ReturnsBadRequest(string title, long guid, string summary, string image, string keywords, string text)
        {
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            var newsController = new NewsController(newsPostsRepoMock.Object, null);
            var newsPost = new NewsPost
            {
                Title = title,
                Guid = guid,
                Summary = summary,
                Image = image,
                Keywords = keywords,
                Text = text
            };

            TestHelper.SimulateValidation(newsController, newsPost);
            var result = newsController.PostNews(newsPost);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData("a", 1, "a", "a", "a", "a")]
        void PostNews_ValidModel_ReturnsOk(string title, long guid, string summary, string image, string keywords, string text)
        {
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            var newsController = new NewsController(newsPostsRepoMock.Object, null);
            var newsPost = new NewsPost
            {
                Title = title,
                Guid = guid,
                Summary = summary,
                Image = image,
                Keywords = keywords,
                Text = text
            };

            TestHelper.SimulateValidation(newsController, newsPost);
            var result = newsController.PostNews(newsPost);

            Assert.IsType<OkResult>(result);
        }
    }
}
