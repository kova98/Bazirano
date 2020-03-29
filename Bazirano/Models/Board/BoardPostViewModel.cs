using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.Board
{
    public class BoardPostViewModel
    {
        public BoardThread ParentThread { get; set; }

        public BoardPost Post { get; set; }
    }
}
