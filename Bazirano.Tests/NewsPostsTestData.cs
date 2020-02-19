using Bazirano.Models.News;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bazirano.Tests
{
    class NewsPostsTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { NewsPosts };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private NewsPost[] NewsPosts => new NewsPost[]
        {
            new NewsPost { Id = 0, DatePosted = DateTime.Now.AddHours(-1) },
            new NewsPost { Id = 1, DatePosted = DateTime.Now.AddHours(-5) },
            new NewsPost { Id = 2, DatePosted = DateTime.Now.AddHours(1) },
            new NewsPost { Id = 3, DatePosted = DateTime.Now.AddHours(-2) },
            new NewsPost { Id = 4, DatePosted = DateTime.Now.AddHours(-10) }
        };
    }
}
