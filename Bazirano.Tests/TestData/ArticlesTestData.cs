using Bazirano.Models.News;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bazirano.Tests.TestData
{
    class ArticlesTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { Articles };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private Article[] Articles => new Article[]
        {
            new Article { Id = 0, DatePosted = DateTime.Now.AddHours(-1) },
            new Article { Id = 1, DatePosted = DateTime.Now.AddHours(-5) },
            new Article { Id = 2, DatePosted = DateTime.Now.AddHours(1) },
            new Article { Id = 3, DatePosted = DateTime.Now.AddHours(-2) },
            new Article { Id = 4, DatePosted = DateTime.Now.AddHours(-10) }
        };
    }
}
