using Bazirano.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.Column
{
    public class Column
    {
        public string Author { get; set; }
        public DateTime DatePosted { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
