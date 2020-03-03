using Bazirano.Models.Board;
using Bazirano.Models.Column;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bazirano.Tests.TestData
{
    public class BoardThreadTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { BoardThreads };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public BoardThread[] BoardThreads => new BoardThread[]
        {
            new BoardThread
            {
                Id = 0,
                Posts = new List<BoardPost>
                {
                    new BoardPost { Id = 0, DatePosted = DateTime.Now },
                    new BoardPost { Id = 1, DatePosted = DateTime.Now },
                    new BoardPost { Id = 2, DatePosted = DateTime.Now },
                }
            },
            new BoardThread
            {
                Id = 1,
                Posts = new List<BoardPost>
                {
                    new BoardPost { Id = 3, DatePosted = DateTime.Now },
                    new BoardPost { Id = 4, DatePosted = DateTime.Now.AddHours(1) },
                    new BoardPost { Id = 5, DatePosted = DateTime.Now },
                }
            },
            new BoardThread
            {
                Id = 2,
                Posts = new List<BoardPost>
                {
                    new BoardPost { Id = 6, DatePosted = DateTime.Now },
                    new BoardPost { Id = 7, DatePosted = DateTime.Now.AddHours(-1) },
                    new BoardPost { Id = 8, DatePosted = DateTime.Now },
                }
            },
        };
    }
}
