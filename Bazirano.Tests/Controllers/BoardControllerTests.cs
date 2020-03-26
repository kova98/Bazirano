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

namespace Bazirano.Tests.Controllers
{
    public class BoardControllerTests
    {
        BoardController GetMockBoardController(BoardThread[] boardThreads)
        {
            var mock = new Mock<IBoardThreadsRepository>();
            mock.Setup(x => x.BoardThreads).Returns(boardThreads.AsQueryable);

            var googleRecaptchaHelperMock = new Mock<IGoogleRecaptchaHelper>();
            googleRecaptchaHelperMock.Setup(x => x.VerifyRecaptcha(It.IsAny<HttpRequest>(), It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);
            
            var writerMock = new Mock<IWriter>();

            var boardController = new BoardController(mock.Object, googleRecaptchaHelperMock.Object, writerMock.Object);

            //boardController.Request.Form["g-recaptcha-response"] = "";

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

        [Theory]
        [ClassData(typeof(BoardThreadTestData))]
        public async Task Respond_ThreadId_DisplaysThreadViewWithCorrectModel(BoardThread[] boardThreads)
        {
            var boardController = GetMockBoardController(boardThreads);
            var respondViewModel = new BoardRespondViewModel { ThreadId = 1, BoardPost = new BoardPost { Text = "", Image = "" } };

            var result = (ViewResult)await boardController.Respond(respondViewModel, null); 
            var model = (BoardThread)result.Model;

            Assert.Equal(nameof(boardController.Thread), result.ViewName);
            Assert.Equal(1, model.Id); 
        }

        [Fact]
        public async void CreateThread_InvalidModel_DisplaysSubmitView()
        {
            var boardThreadsRepoMock = new Mock<IBoardThreadsRepository>();
            var boardController = new BoardController(boardThreadsRepoMock.Object, new RecaptchaMock(), null);
            var boardPost = new BoardPost();

            TestHelper.SimulateValidation(boardController, boardPost);
            var result = (ViewResult)await boardController.CreateThread(boardPost, null);

            Assert.Equal(nameof(boardController.Submit), result.ViewName);
        }

        [Theory]
        [InlineData(40, 0)]
        [InlineData(41, 1)]
        public async void CreateThread_ValidModel_DisplaysThreadViewAndCallsRemoveLastThread(int threadCount, int timesRemoveThreadCalled)
        {
            var boardPost = new BoardPost { Id = 1, Text = "test test test" };
            var boardThreadToAdd = new BoardThread { Posts = new List<BoardPost> { boardPost } };
            var oldestThread = new BoardThread { Posts = new List<BoardPost> { new BoardPost { DatePosted = DateTime.Now.AddHours(-1) } } };
            var boardThreads = GetBoardThreadsListContaining(threadCount, boardThreadToAdd, oldestThread );

            var boardThreadsRepoMock = new Mock<IBoardThreadsRepository>();
            boardThreadsRepoMock.Setup(x => x.BoardThreads).Returns(boardThreads.AsQueryable());

            var boardController = new BoardController(boardThreadsRepoMock.Object, new RecaptchaMock(), null);

            TestHelper.SimulateValidation(boardController, boardPost);
            var result = (ViewResult)await boardController.CreateThread(boardPost, null);

            Assert.Equal(nameof(boardController.Thread), result.ViewName);
            boardThreadsRepoMock.Verify(x => x.RemoveThread(oldestThread.Id), Times.Exactly(timesRemoveThreadCalled));
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
