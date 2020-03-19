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

        public string ColumnImage { get; set; }

        public string ColumnTitle { get; set; }

        public string ColumnText { get; set; }

        public Author Author { get; set; }

        public ColumnRequestStatus Status { get; set; }

        public DateTime DateRequested { get; set; }

        public DateTime DateApproved { get; set; }

        public long AdminApprovedId { get; set; }

        public string AuthorRemarks { get; set; }

        public string AdminRemarks { get; set; }
    }
}
