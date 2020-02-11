using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.Column
{
    public class ColumnMainPageViewModel
    {
        public ColumnPost FirstColumn { get; set; }

        public List<ColumnPost> Columns { get; set; }

        public List<Author> Authors { get; set; }
    }
}
