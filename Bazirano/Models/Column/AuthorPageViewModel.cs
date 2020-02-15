using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.Column
{
    public class AuthorPageViewModel
    {
        public Author Author { get; set; }

        public List<ColumnPost> Columns { get; set; }
    }
}
