using System.Collections.Generic;

namespace Bazirano.Models.Board
{
    /// <summary>
    /// The board thread model.
    /// </summary>
    public class BoardThread
    {
        /// <summary>
        /// The board thread Id.
        /// </summary>
        public long Id { get; set; }

        public bool SafeForWork { get; set; }

        /// <summary>
        /// The value indicating whether the thread is locked or not. Defaulted to false.
        /// </summary>
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// The number of posts in the thread.
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        /// The number of images in the thread.
        /// </summary>
        public int ImageCount { get; set; }

        /// <summary>
        /// The collection of all the <see cref="BoardPost"/>s in the thread.
        /// </summary>
        public ICollection<BoardPost> Posts { get; set; }
    }
}
