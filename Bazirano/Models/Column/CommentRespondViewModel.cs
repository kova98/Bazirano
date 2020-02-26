using Bazirano.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.Column
{
    public class CommentRespondViewModel
    {
        public long CommentId { get; set; }

        public Comment Comment { get; set; }

        public string ReturnUrl { get; set; }
    }
}
