using Bazirano.Models.Column;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bazirano.Tests.TestData
{
    public class ColumnTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { Columns, Authors };
        }

        public static IEnumerator<object[]> GetAuthors()
        {
            yield return new object[] { Authors };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static ColumnPost[] Columns => new ColumnPost[]
        {
            new ColumnPost { Id = 0, DatePosted = DateTime.Now.AddHours(-1) , Author = new Author{ Id = 0 } },
            new ColumnPost { Id = 1, DatePosted = DateTime.Now.AddHours(1) , Author = new Author{ Id = 1 }},
            new ColumnPost { Id = 2, DatePosted = DateTime.Now, Author = new Author{ Id = 1 }},
        };

        private static Author[] Authors => new Author[]
        {
            new Author { Id = 0 },
            new Author { Id = 1 },
            new Author { Id = 2 }
        };
    }
}
