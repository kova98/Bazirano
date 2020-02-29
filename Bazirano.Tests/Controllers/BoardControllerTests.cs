using Bazirano.Controllers;
using Bazirano.Models.Board;
using Bazirano.Models.DataAccess;
using Bazirano.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Specialized;

namespace Bazirano.Tests.Controllers
{
    public class BoardControllerTests
    {
        BoardController GetMockBoardController(BoardThread[] boardThreads)
        {
            var mock = new Mock<IBoardThreadsRepository>();
            mock.Setup(x => x.BoardThreads).Returns(boardThreads.AsQueryable);

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(x => x["GoogleReCaptcha:secret"]).Returns("");

            var googleRecaptchaHelperMock = new Mock<IGoogleRecaptchaHelper>();
            googleRecaptchaHelperMock.Setup(x => x.IsRecaptchaValid(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            var writerMock = new Mock<IWriter>();

            var boardController = new BoardController(mock.Object, configMock.Object, googleRecaptchaHelperMock.Object, writerMock.Object);

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
        public async void Respond_ThreadId_DisplaysViewWithCorrectModel(BoardThread[] boardThreads)
        {
            var boardController = GetMockBoardController(boardThreads);
            var respondViewModel = new BoardRespondViewModel { ThreadId = 1 };

            var test = await boardController.Respond(respondViewModel, null);
            var result = (ViewResult)test;
            var model = (BoardThread)result.Model;

            Assert.Equal(nameof(boardController.Thread), result.ViewName);
            Assert.Equal(1, model.Id); ;
        }

    }
}
