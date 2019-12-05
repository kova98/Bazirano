using Bazirano.Models.Shared;
using System;
using System.Collections.Generic;

namespace Bazirano.Models.Column
{
    /// <summary>
    /// The column model.
    /// </summary>
    public class Column
    {
        /// <summary>
        /// The author of the column.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The date which the column was posted on.
        /// </summary>
        public DateTime DatePosted { get; set; }

        /// <summary>
        /// The collection of all the <see cref="Comment"/>s on the column.
        /// </summary>
        public ICollection<Comment> Comments { get; set; }
    }
}
