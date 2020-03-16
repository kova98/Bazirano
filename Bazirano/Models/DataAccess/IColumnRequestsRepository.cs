using Bazirano.Models.AuthorInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.DataAccess
{
    public interface IColumnRequestsRepository
    {
        IQueryable<ColumnRequest> ColumnRequests { get; }

        void AddColumnRequest(ColumnRequest columnRequest);

        void RemoveColumnRequest(long columnRequestId);

        void EditColumnRequest(ColumnRequest columnRequest);
    }
}
