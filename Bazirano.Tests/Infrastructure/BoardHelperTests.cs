using Bazirano.Controllers;
using Bazirano.Infrastructure;
using Bazirano.Models.Board;
using Bazirano.Models.DataAccess;
using Bazirano.Tests.TestData;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Bazirano.Tests.Infrastructure
{
    public class BoardHelperTests
    {
        [Theory]
        [ClassData(typeof(BoardThreadTestData))]
        void SortByBumpOrder_CanSort(BoardThread[] boardThreads)
        {
            var boardThreadsList = boardThreads.ToList();

            var sortedBoardThreads = boardThreadsList.SortByBumpOrder();

            Assert.Equal(1, sortedBoardThreads.First().Id);
        }
    }
}
