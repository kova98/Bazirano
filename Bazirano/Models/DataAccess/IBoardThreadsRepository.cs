using System.Linq;

namespace Bazirano.Models.DataAccess
{
    public interface IBoardThreadsRepository
    {
        IQueryable<BoardThread> BoardThreads { get; }
        void AddThread(BoardThread thread);
        void RemoveThread(BoardThread thread);
    }
}
