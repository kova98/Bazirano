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

namespace Bazirano.Tests.Controllers
{
    public class BoardControllerTests
    {
        BoardController GetMockBoardController(BoardThread[] boardThreads)
        {
            var mock = new Mock<IBoardThreadRepository>();
            mock.Setup(x => x.BoardThreads).Returns(boardThreads.AsQueryable);

            var googleRecaptchaHelperMock = new Mock<IGoogleRecaptchaHelper>();
            googleRecaptchaHelperMock.Setup(x => x.VerifyRecaptcha(It.IsAny<HttpRequest>(), It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);
            
            var writerMock = new Mock<IWriter>();

            var boardController = new BoardController(mock.Object, googleRecaptchaHelperMock.Object, writerMock.Object)
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

        [Fact]
        public async void CreateThread_InvalidModel_DisplaysSubmitView()
        {
            var boardController = new BoardController(Mock.Of<IBoardThreadRepository>(), new RecaptchaMock(), null);
            var boardPost = new BoardPost();

            TestHelper.SimulateValidation(boardController, boardPost);
            var result = (ViewResult)await boardController.CreateThread(boardPost, null);

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
            var boardController = new BoardController(boardThreadsRepoMock.Object, new RecaptchaMock(), null);

            await boardController.CreateThread(new BoardPost { Text = "test" }, null);

            boardThreadsRepoMock.Verify(x => x.RemoveThread(oldestThread.Id), Times.Exactly(timesRemoveThreadCalled));
        }

        [Fact]
        public async void CreateThread_ValidModel_RedirectsToThread()
        {
            var boardPost = new BoardPost { Id = 1, Text = "test test test" };
            var boardController = new BoardController(Mock.Of<IBoardThreadRepository>(), new RecaptchaMock(), null);

            TestHelper.SimulateValidation(boardController, boardPost);
            var result = (RedirectToActionResult)await boardController.CreateThread(boardPost, null);

            Assert.Equal("Thread", result.ActionName);
            Assert.Equal("Board", result.ControllerName);
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
