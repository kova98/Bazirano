﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Board.Models
{
    public class BoardRespondViewModel
    {
        public long ThreadId { get; set; }
        public BoardPost BoardPost { get; set; }
    }
}