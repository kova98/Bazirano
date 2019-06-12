using Bazirano.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Bazirano.Tests
{
    public class NewsHelperTests
    {
        [Fact]
        public void Can_Get_Time_Elapsed_With_Days()
        {
            // Arrange
            TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);
            TimeSpan moreDays = new TimeSpan(4, 0, 0, 0);
            TimeSpan endsWith1 = new TimeSpan(3561, 0, 0, 0);
            TimeSpan notEndsWith1 = new TimeSpan(3564, 0, 0, 0);
            TimeSpan elevenDays = new TimeSpan(11, 0, 0, 0);

            // Act
            TimeDisplay actualOneDay = NewsHelper.GetTimeElapsed(oneDay);
            TimeDisplay actualMoreDays = NewsHelper.GetTimeElapsed(moreDays);
            TimeDisplay actualEndsWith1 = NewsHelper.GetTimeElapsed(endsWith1);
            TimeDisplay actualNotEndsWith1 = NewsHelper.GetTimeElapsed(notEndsWith1);
            TimeDisplay actualEndsWith11 = NewsHelper.GetTimeElapsed(elevenDays);

            // Assert
            Assert.Equal(1, actualOneDay.Number);
            Assert.Equal("dan", actualOneDay.Text);

            Assert.Equal(4, actualMoreDays.Number);
            Assert.Equal("dana", actualMoreDays.Text);

            Assert.Equal(3561, actualEndsWith1.Number);
            Assert.Equal("dan", actualEndsWith1.Text);

            Assert.Equal(3564, actualNotEndsWith1.Number);
            Assert.Equal("dana", actualNotEndsWith1.Text);

            Assert.Equal(11, actualEndsWith11.Number);
            Assert.Equal("dana", actualEndsWith11.Text);
        }

        [Fact]
        public void Can_Get_Time_Elapsed_With_Hours()
        {
            // Arrange
            TimeSpan oneHour = new TimeSpan(1, 0, 0);
            TimeSpan moreHours = new TimeSpan(4, 0, 0);
            TimeSpan endsWith1 = new TimeSpan(21, 0, 0);
            TimeSpan notEndsWith1 = new TimeSpan(14, 0, 0);
            TimeSpan elevenHours = new TimeSpan(11, 0, 0);

            // Act
            TimeDisplay actualOneHour = NewsHelper.GetTimeElapsed(oneHour);
            TimeDisplay actualMoreHours = NewsHelper.GetTimeElapsed(moreHours);
            TimeDisplay actualEndsWith1 = NewsHelper.GetTimeElapsed(endsWith1);
            TimeDisplay actualNotEndsWith1 = NewsHelper.GetTimeElapsed(notEndsWith1);
            TimeDisplay actualEndsWith11 = NewsHelper.GetTimeElapsed(elevenHours);

            // Assert
            Assert.Equal(1, actualOneHour.Number);
            Assert.Equal("sat", actualOneHour.Text);

            Assert.Equal(4, actualMoreHours.Number);
            Assert.Equal("sati", actualMoreHours.Text);

            Assert.Equal(21, actualEndsWith1.Number);
            Assert.Equal("sat", actualEndsWith1.Text);

            Assert.Equal(14, actualNotEndsWith1.Number);
            Assert.Equal("sati", actualNotEndsWith1.Text);

            Assert.Equal(11, actualEndsWith11.Number);
            Assert.Equal("sati", actualEndsWith11.Text);
        }

        [Fact]
        public void Can_Get_Time_Elapsed_With_Minutes()
        {
            // Arrange
            TimeSpan minutes = new TimeSpan(0, 13, 0);

            // Act
            TimeDisplay actual = NewsHelper.GetTimeElapsed(minutes);

            // Assert
            Assert.Equal(13, actual.Number);
            Assert.Equal("min", actual.Text);
        }
    }
}
