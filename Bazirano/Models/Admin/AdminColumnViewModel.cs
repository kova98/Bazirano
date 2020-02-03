using Bazirano.Models.Column;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.Admin
{
    public class AdminColumnViewModel
    {
        public List<ColumnPost> ColumnPosts { get; set; }

        public List<Author> Authors { get; set; }
    }
}
