using Bazirano.Models.News;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public interface INewsHelper
    {
        Task<NewsPageViewModel> GetCurrentNewsAsync();
        Task<List<NewsPost>> GetLastXPosts(int x);
    }
}
