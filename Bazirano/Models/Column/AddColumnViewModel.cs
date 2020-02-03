using System.Collections.Generic;

namespace Bazirano.Models.Column
{
    public class AddColumnViewModel
    {
        public ColumnPost Column { get; set; }

        public List<Author> Authors { get; set; }
    }
}
