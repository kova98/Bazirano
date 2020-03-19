using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.AuthorInterface
{
    public class ColumnRequestsOverviewViewModel
    {
        public List<ColumnRequest> DraftRequests { get; set; }
        public List<ColumnRequest> PendingRequests { get; set; }
        public List<ColumnRequest> RejectedRequests { get; set; }
        public List<ColumnRequest> ApprovedRequests { get; set; }

    }
}
