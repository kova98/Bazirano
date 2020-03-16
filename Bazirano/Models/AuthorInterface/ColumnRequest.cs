using Bazirano.Models.Column;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.AuthorInterface
{
    public class ColumnRequest
    {
        public long Id { get; set; }

        [Required]
        public ColumnPost Column { get; set; }

        public DateTime TimeRequested { get; set; }

        public DateTime TimeApproved { get; set; }

        public long AdminApprovedId { get; set; }

        public string AuthorRemarks { get; set; }

        public string AdminRemarks { get; set; }
    }
}
