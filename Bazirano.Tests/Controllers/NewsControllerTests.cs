using Bazirano.Controllers;
using Bazirano.Models.Admin;
using Bazirano.Models.Board;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Bazirano.Models.Home;
using Bazirano.Models.News;
using Bazirano.Models.Shared;
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
            var newsController = new NewsController(newsPostsRepoMock.Object);

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
            var newsController = new NewsController(newsPostsRepoMock.Object);

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
            var newsController = new NewsController(newsPostsRepoMock.Object);

            var result = (RedirectToActionResult)newsController.Article(0);

            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("Article", result.ActionName);
        }

        [Fact]
        void PostComment_ValidViewModel_DisplaysViewWithProperViewModelAndAddsComment()
        {
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            var newsController = new NewsController(newsPostsRepoMock.Object);
            var viewModel = new ArticleRespondViewModel
            {
                ArticleId = 1,
                Comment = new Comment
                {
                    Id = 1,
                    Text = "  test  "
                }
            };

            var result = (RedirectToActionResult)newsController.PostComment(viewModel);

            Assert.Equal(nameof(newsController.Article), result.ActionName);
            Assert.Equal("test", viewModel.Comment.Text);

            newsPostsRepoMock.Verify(x => x.AddCommentToNewsPost(viewModel.Comment, viewModel.ArticleId));
        }

        [Fact]
        void PostNews_DistinctGuid_AddsNewsPost()
        {
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            newsPostsRepoMock.Setup(x => x.NewsPosts).Returns(new NewsPost[] { new NewsPost { Guid = 1 } }.AsQueryable());
            var newsController = new NewsController(newsPostsRepoMock.Object);
            var newsPost = new NewsPost { Guid = 2 };

            newsController.PostNews(newsPost);

            newsPostsRepoMock.Verify(x=>x.AddNewsPost(newsPost));
        }

        [Fact]
        void PostNews_ExistingGuid_AddsNewsPost()
        {
            var newsPostsRepoMock = new Mock<INewsPostsRepository>();
            newsPostsRepoMock.Setup(x => x.NewsPosts).Returns(new NewsPost[] { new NewsPost { Guid = 1 } }.AsQueryable());
            var newsController = new NewsController(newsPostsRepoMock.Object);
            var newsPost = new NewsPost { Guid = 1 };

            newsController.PostNews(newsPost);

            newsPostsRepoMock.Verify(x => x.EditNewsPost(newsPost));
        }
    }
}
