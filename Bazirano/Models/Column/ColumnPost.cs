using Bazirano.Models.Shared;
using System;
using System.Collections.Generic;

namespace Bazirano.Models.Column
{
    /// <summary>
    /// The column model.
    /// </summary>
    public class ColumnPost
    {
        /// <summary>
        /// The id of the column.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The author of the column.
        /// </summary>
        public Author Author { get; set; }

        /// <summary>
        /// The url for the header image.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The title of the column.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The text of the column, in raw HTML.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The date which the column was posted on.
        /// </summary>
        public DateTime DatePosted { get; set; }

        /// <summary>
        /// The collection of all the <see cref="Comment"/>s on the column.
        /// </summary>
        public ICollection<Comment> Comments { get; set; }

        public string FirstParagraph
        {
            get
            {
                if (string.IsNullOrEmpty(Text))
                {
                    return "";
                }

                int startIndex = Text.IndexOf("<p>") + 3;
                int endIndex = Text.IndexOf("</p>") - 1;    
                int length = endIndex - startIndex;
                return $"{Text.Substring(startIndex, length).Trim()}..";
            }
        }
    }
}
