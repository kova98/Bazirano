using Bazirano.Infrastructure;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Bazirano.Tests
{
    public class NewsHelperTests
    {
        [Fact]
        void Can_Get_NewsPageViewModel()
        {
            // Arrange
            Mock<INewsPostsRepository> mock = new Mock<INewsPostsRepository>();
            mock.Setup(x => x.NewsPosts).Returns((new NewsPost[]
            {
                new NewsPost{ Id = 0, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 1},
                new NewsPost{ Id = 1, DatePosted = DateTime.Now.AddHours(-4), ViewCount = 5},
                new NewsPost{ Id = 2, DatePosted = DateTime.Now.AddHours(-5), ViewCount = 10},
                new NewsPost{ Id = 3, DatePosted = DateTime.Now, ViewCount = 1},
                new NewsPost{ Id = 4, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 1},
                new NewsPost{ Id = 5, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 1},
                new NewsPost{ Id = 6, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 20},
                new NewsPost{ Id = 7, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 1},
                new NewsPost{ Id = 8, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 1},
                new NewsPost{ Id = 9, DatePosted = DateTime.Now.AddHours(-1), ViewCount = 1}
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
