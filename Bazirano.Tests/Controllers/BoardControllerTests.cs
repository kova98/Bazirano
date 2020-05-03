using Bazirano.Controllers;
using Bazirano.Models.Board;
using Bazirano.Models.DataAccess;
using Bazirano.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Threading.Tasks;
using Bazirano.Tests.TestData;
using Bazirano.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Bazirano.Models.News;

namespace Bazirano.Tests.Controllers
{
    public class BoardControllerTests
    {
        BoardController GetMockBoardController(BoardThread[] boardThreads = null, Article[] articles = null)
        {
            boardThreads ??= new BoardThread[] { };
            articles ??= new Article[] { };

            var boardRepoMock = new Mock<IBoardThreadRepository>();
            boardRepoMock.Setup(x => x.BoardThreads).Returns(boardThreads.AsQueryable);

            var articleRepoMock = new Mock<IArticleRepository>();
            articleRepoMock.Setup(x => x.Articles).Returns(articles.AsQueryable);

            var googleRecaptchaHelperMock = new Mock<IGoogleRecaptchaHelper>();
            googleRecaptchaHelperMock.Setup(x => x.VerifyRecaptcha(It.IsAny<HttpRequest>(), It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);
            
            var writerMock = new Mock<IWriter>();
            var loggerMock = new Mock<ILogger<BoardController>>();

            var boardController = new BoardController(
                boardRepoMock.Object,
                articleRepoMock.Object,
                googleRecaptchaHelperMock.Object,
                writerMock.Object,
                loggerMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            return boardController;
        }

        [Fact]
        public void Submit_DisplaysView()
        {
            var boardController = GetMockBoardController(null);

            var result = (ViewResult)boardController.Submit();

            Assert.Equal(nameof(boardController.Submit), result.ViewName);
        }

        [Theory]
        [ClassData(typeof(BoardThreadTestData))]
        public void Catalog_DisplaysView(BoardThread[] boardThreads)
        {
            var boardController = GetMockBoardController(boardThreads);

            var result = (ViewResult)boardController.Catalog();
            var model = (List<BoardThread>)result.Model;

            Assert.Equal(nameof(boardController.Catalog), result.ViewName);
            Assert.Equal(1, model.First().Id);
        }

        [Theory]
        [ClassData(typeof(BoardThreadTestData))]
        public void Thread_DisplaysViewWithCorrectModel(BoardThread[] boardThreads)
        {
            var boardController = GetMockBoardController(boardThreads);

            var result = (ViewResult)boardController.Thread(1);
            var model = (BoardThread)result.Model;

            Assert.Equal(nameof(boardController.Thread), result.ViewName);
            Assert.Equal(1, model.Id); ;
        }

        [Fact]
        public async Task Respond_ThreadId_RedirectsToThread()
        {
            var boardController = GetMockBoardController(new BoardThread[] { new BoardThread { Id = 1, ImageCount = 0 } });
            var respondViewModel = new BoardRespondViewModel { ThreadId = 1, BoardPost = new BoardPost { Text = "", Image = "" } };

            var result = (RedirectToActionResult)await boardController.Respond(respondViewModel, null); 

            Assert.Equal("Thread", result.ActionName);
            Assert.Equal("Board", result.ControllerName);
        }

        [Theory]
        [ClassData(typeof(BoardThreadTestData))]
        public async Task Respond_ThreadId_DisplaysThreadViewWithCorrectModel(BoardThread[] boardThreads)
        {
            var boardController = GetMockBoardController(boardThreads);
            var respondViewModel = new BoardRespondViewModel { ThreadId = 1, BoardPost = new BoardPost { Text = "", Image = "" } };

            var result = (RedirectToActionResult)await boardController.Respond(respondViewModel, null);

            Assert.True(result.RouteValues.ContainsKey("Id"));
            result.RouteValues.TryGetValue("Id", out object id);
            Assert.Equal(1L, id);
        }

        [Theory]
        [InlineData("valid text", "invalid url")]
        [InlineData("invalid", "http://valid.url")]
        [InlineData("invalid", "invalid")]
        public async void CreateThread_InvalidModel_DisplaysSubmitView(string text, string imageUrl)
        {
            var boardController = GetMockBoardController();
            var viewModel = new SubmitViewModel { Text = text, ImageUrl = imageUrl };

            TestHelper.SimulateValidation(boardController, viewModel);
            var result = (ViewResult)await boardController.CreateThread(viewModel, null);

            Assert.Equal(nameof(boardController.Submit), result.ViewName);
        }

        [Theory]
        [InlineData(40, 0)]
        [InlineData(41, 1)]
        public async void CreateThread_MoreThan40Threads_CallsRemoveThread(int threadCount, int timesRemoveThreadCalled)
        {
            var oldestThread = new BoardThread { Id = 1, Posts = new List<BoardPost> { new BoardPost { DatePosted = DateTime.Now.AddHours(-1) } } };
            var boardThreads = GetBoardThreadsListContaining(threadCount, oldestThread );
            var boardThreadsRepoMock = new Mock<IBoardThreadRepository>();
            boardThreadsRepoMock.Setup(x => x.BoardThreads).Returns(boardThreads.AsQueryable());
            var boardController = new BoardController(boardThreadsRepoMock.Object, null, new RecaptchaMock(), null, null);

            await boardController.CreateThread(new SubmitViewModel { Text = "test" }, null);

            boardThreadsRepoMock.Verify(x => x.RemoveThread(oldestThread.Id), Times.Exactly(timesRemoveThreadCalled));
        }

        [Theory]
        [InlineData("valid text", null)]
        [InlineData("valid text", "http://valid.png")]
        public async void CreateThread_ValidModel_RedirectsToThread(string text, string imageUrl)
        {
            var boardPost = new SubmitViewModel { Text = text, ImageUrl = imageUrl };
            var boardController = GetMockBoardController();

            TestHelper.SimulateValidation(boardController, boardPost);
            var result = (RedirectToActionResult)await boardController.CreateThread(boardPost, null);

            Assert.Equal("Thread", result.ActionName);
            Assert.Equal("Board", result.ControllerName);
        }

        [Fact]
        void StartDiscussion_ArticleDoesNotExist_RedirectsToError()
        {
            var articles = new Article[] { new Article { Id = 1 } };
            var boardController = GetMockBoardController(articles: articles);

            var result = (RedirectToActionResult)boardController.StartDiscussion(2);

            Assert.Equal("Article", result.ActionName);
            Assert.Equal("Error", result.ControllerName);
        }

        [Fact]
        void StartDiscussion_DiscussionExists_RedirectsToThread()
        {
            var articles = new Article[] { new Article { Id = 1, Discussion = new BoardThread { Id = 2 } } };
            var boardController = GetMockBoardController(articles: articles);

            var result = (RedirectToActionResult)boardController.StartDiscussion(1);

            Assert.Equal("Thread", result.ActionName);
            Assert.Equal("Board", result.ControllerName);
            Assert.Equal(2L, result.RouteValues["id"]);
        }

        [Fact]
        void StartDiscussion_DiscussionDoesNotExist_CreatesThread()
        {
            var articleRepoMock = new Mock<IArticleRepository>();
            var articles = new Article[]
            {
                new Article
                {
                    Id = 1,
                    Image = "test1",
                    Title = "test2",
                    SourceUrl = "test3"
                }
            };
            articleRepoMock.Setup(x => x.Articles).Returns(articles.AsQueryable);
            var boardController = new BoardController(Mock.Of<IBoardThreadRepository>(), articleRepoMock.Object, null, null, null);

            var result = (RedirectToActionResult)boardController.StartDiscussion(1);

            articleRepoMock.Verify(x => x.EditArticle(
                It.Is<Article>(t =>
                    (t.Discussion.Posts.First().Image == "test1") &&
                    (t.Discussion.Posts.First().Text == "test2") &&
                    (t.Discussion.SourceUrl == "test3"))),
                Times.Once);
        }

        [Fact]
        void StartDiscussion_CreatesThread()
        {
            var articles = new Article[] { new Article { Id = 1, Discussion = new BoardThread { Id = 2 } } };
            var boardController = GetMockBoardController(articles: articles);

            var result = (RedirectToActionResult)boardController.StartDiscussion(1);

            Assert.Equal("Thread", result.ActionName);
            Assert.Equal("Board", result.ControllerName);
            Assert.Equal(2L, result.RouteValues["id"]);
        }

        private List<BoardThread> GetBoardThreadsListContaining(int threadCount, params BoardThread[] threads)
        {
            var boardThreads = new List<BoardThread>();
            boardThreads.AddRange(threads);

            while (boardThreads.Count < threadCount)
            {
                boardThreads.Add(new BoardThread { Posts = new List<BoardPost> { new BoardPost { DatePosted = DateTime.Now } } });
            }

            return boardThreads;
        }
    }
}
