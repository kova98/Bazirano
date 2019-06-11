using System;

namespace Bazirano.Models.News
{
    public class NewsComment
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
        public DateTime DatePosted { get; set; }
    }
}
