using Bazirano.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
using System.Linq;

namespace Bazirano.Tests
{
    public class NewsHelperTests
    {
        [Theory]
        [InlineData(1, "dan")]
        [InlineData(4, "dana")]
        [InlineData(3561, "dan")]
        [InlineData(3564, "dana")]
        [InlineData(11, "dana")]
        public void Can_Get_Time_Elapsed_With_Days(int days, string noun)
        {
            // Arrange
            TimeSpan time = new TimeSpan(days, 0, 0, 0);

            // Act
            TimeDisplay actualTimeDisplay = TimeHelper.GetTimeDisplayFromTimeElapsed(time);

            // Assert
            Assert.Equal(days, actualTimeDisplay.Number);
            Assert.Equal(noun, actualTimeDisplay.Text);
        }

        [Theory]
        [InlineData(1, "sat")]
        [InlineData(4, "sata")]
        [InlineData(21, "sat")]
        [InlineData(14, "sati")]
        [InlineData(11, "sati")]
        public void Can_Get_Time_Display_With_Hours(int hours, string noun)
        {
            // Arrange
            TimeSpan time = new TimeSpan(hours, 0, 0);

            // Act
            TimeDisplay actualTimeDisplay = TimeHelper.GetTimeDisplayFromTimeElapsed(time);

            // Assert
            Assert.Equal(hours, actualTimeDisplay.Number);
            Assert.Equal(noun, actualTimeDisplay.Text);
        }

        [Fact]
        public void Can_Get_Time_Display_With_Minutes()
        {
            // Arrange
            TimeSpan minutes = new TimeSpan(0, 13, 0);

            // Act
            TimeDisplay actual = TimeHelper.GetTimeDisplayFromTimeElapsed(minutes);

            // Assert
            Assert.Equal(13, actual.Number);
            Assert.Equal("min", actual.Text);
        }

        [Fact]
        public void Can_Get_Main_Post_Related_Posts()
        {
            // Arrange
            Mock<INewsPostsRepository> mock = new Mock<INewsPostsRepository>();
            mock.Setup(x => x.NewsPosts).Returns((new NewsPost[]
            {
                new NewsPost{ KeywordsList = new List<string>{ "keyword1", "keyword6", "keyword7"} },
                new NewsPost{ KeywordsList = new List<string>{ "keyword1", "keyword2" } },
                new NewsPost{ KeywordsList = new List<string>{ "keyword1", "keyword2", "keyword3", "keyword6", "keyword7" } },
                new NewsPost{ KeywordsList = new List<string>{ "keyword1", "keyword7", "keyword8", "keyword9", "keyword10" } },
                new NewsPost{ KeywordsList = new List<string>{ "keyword7", "keyword9", "keyword10" } },
            })
            .AsQueryable());

            NewsPost mainPost = new NewsPost
            {
                KeywordsList = new List<string> { "keyword1", "keyword2", "keyword3", "keyword4", "keyword5" }
            };

            // Act
            var matches = mock.Object.NewsPosts
                .Where(p => p.KeywordsList.KeywordMatches(mainPost.KeywordsList) > 1)
                .ToList();

            // Assert
            Assert.Equal(2, matches.Count);
            Assert.Equal(2, matches[0].KeywordsList.Count);
            Assert.Equal(5, matches[1].KeywordsList.Count);
        }
    }
}
