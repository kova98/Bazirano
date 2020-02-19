using Bazirano.Models.Column;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bazirano.Tests
{
    public class ColumnTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { Columns, Authors };
        }

        public static IEnumerator<object[]> GetAuthors()
        {
            var instance = new ColumnTestData();
            yield return new object[] { instance.Authors };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private ColumnPost[] Columns => new ColumnPost[]
        {
            new ColumnPost { Id = 0 },
            new ColumnPost { Id = 1 },
            new ColumnPost { Id = 2 },
        };

        private Author[] Authors => new Author[]
        {
            new Author { Id = 0 },
            new Author { Id = 1 },
            new Author { Id = 2 }
        };
    }
}
