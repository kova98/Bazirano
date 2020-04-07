using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Bazirano.Aggregator.Helpers;

namespace Bazirano.Tests.Scraper
{
    public class KeywordHelperTests
    {
        [Fact]
        public void GetKeywordsFromTitle_ReturnsCorrectKeywords()
        {
            var helper = new KeywordHelper();
            var title = "This,; is: a## test title! čžćđ## 123as sdf-;:\"'+? (FOTO) (VIDEO)";

            var keywords = helper.GetKeywordsFromTitle(title);

            Assert.Equal("THIS,TEST,TITLE,ČŽĆĐ", keywords);
        }
    }
}
