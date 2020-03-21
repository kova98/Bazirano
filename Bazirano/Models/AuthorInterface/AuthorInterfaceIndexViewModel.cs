using Bazirano.Models.Column;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.AuthorInterface
{
    public class AuthorInterfaceIndexViewModel
    {
        public Author Author { get; set; }
        public List<ColumnRequest> DraftRequests { get; set; }
        public List<ColumnRequest> PendingRequests { get; set; }
        public List<ColumnRequest> RejectedRequests { get; set; }
        public List<ColumnRequest> ApprovedRequests { get; set; }
    }
}
