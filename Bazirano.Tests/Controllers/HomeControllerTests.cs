using Bazirano.Controllers;
using Bazirano.Models.Admin;
using Bazirano.Models.Board;
using Bazirano.Models.Column;
using Bazirano.Models.DataAccess;
using Bazirano.Models.Home;
using Bazirano.Models.News;
using Bazirano.Tests.TestData;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bazirano.Tests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]  
        void Index_DisplaysView()
        {
            var homeController = new HomeController(Mock.Of<IBoardThreadRepository>(), Mock.Of<IArticleRepository>(), Mock.Of<IColumnRepository>());

            var result = (ViewResult)homeController.Index();

            Assert.Equal("Index", result.ViewName);
        }

        [Fact]
        public void Index_DisplaysSafeForWorkThreads()
        {
            var boardThreadsRepo = Mock.Of<IBoardThreadRepository>(x => x.BoardThreads == new BoardThread[]
            {
                new BoardThread { SafeForWork = true, Posts = new BoardPost[] { new BoardPost() } },
                new BoardThread { SafeForWork = false, Posts = new BoardPost[] { new BoardPost() } }
            }.AsQueryable());

            var controller = new HomeController(boardThreadsRepo, Mock.Of<IArticleRepository>(), Mock.Of<IColumnRepository>());

            var result = (ViewResult)controller.Index();
            var model = (HomePageViewModel)result.Model;

            Assert.Empty(model.Threads.Where(t => t.SafeForWork == false));
        }
    }
}
