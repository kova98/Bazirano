using Bazirano.Models.Column;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.DataAccess
{
    public interface IColumnRepository
    {
        IQueryable<ColumnPost> ColumnPosts { get; }

        IQueryable<Author> Authors { get; }

        void AddColumn(ColumnPost column);

        void SaveColumn(ColumnPost column);

        void SaveAuthor(Author author);
    }
}
