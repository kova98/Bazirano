using Bazirano.Models.News;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public interface INewsHelper
    {
        Task<NewsPageViewModel> GetCurrentNewsAsync();
    }
}
