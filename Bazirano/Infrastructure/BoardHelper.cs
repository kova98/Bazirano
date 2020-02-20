using Bazirano.Models.Board;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public static class BoardHelper
    {
        public static List<BoardThread> SortByBumpOrder(this List<BoardThread> boardThreads)
        {
            return boardThreads
                .OrderByDescending(t => t.Posts
                    .OrderByDescending(p => p.DatePosted)
                        .First().DatePosted)
                .ToList();
        }
    }
}
