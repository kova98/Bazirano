using System.Collections.Generic;

namespace Bazirano.Models.Board
{
    public class BoardThread
    {
        public long Id { get; set; }
        public bool IsLocked { get; set; } = false;
        public int PostCount { get; set; }
        public int ImageCount { get; set; }
        public ICollection<BoardPost> Posts { get; set; }
    }
}
