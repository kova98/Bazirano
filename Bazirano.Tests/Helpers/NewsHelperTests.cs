using Bazirano.Infrastructure;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Bazirano.Tests.Helpers
{
    public class NewsHelperTests
    {
        [Fact]
        void Can_Get_NewsPageViewModel()
        {
            // Arrange
            Mock<IArticleRepository> mock = new Mock<IArticleRepository>();
            mock.Setup(x => x.Articles).Returns((new Article[]
            {
                new Article{ Id = 0, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 1},
                new Article{ Id = 1, DatePosted = DateTime.Now.AddHours(-4), ViewCount = 5},
                new Article{ Id = 2, DatePosted = DateTime.Now.AddHours(-5), ViewCount = 10},
                new Article{ Id = 3, DatePosted = DateTime.Now, ViewCount = 1},
                new Article{ Id = 4, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 1},
                new Article{ Id = 5, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 1},
                new Article{ Id = 6, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 20},
                new Article{ Id = 7, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 1},
                new Article{ Id = 8, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 1},
                new Article{ Id = 9, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 1}
            })
            .AsQueryable());

            NewsHelper newsHelper = new NewsHelper(mock.Object);

            // Act
            NewsPageViewModel actual = newsHelper.GetNewsPageViewModel();

            // Assert
            Assert.Equal(6, actual.MainPost.Id);
            Assert.Equal(2, actual.SecondaryPost.Id);
            Assert.Equal(3, actual.MainPostRelatedPosts.First().Id);
            Assert.Equal(1, actual.PostList.First().Id);
            Assert.Equal(3, actual.LatestPosts.First().Id);
        }
    }
}
