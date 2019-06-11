using Bazirano.Models.News;

namespace Bazirano.Infrastructure
{
    public interface INewsHelper
    {
        NewsPageViewModel CurrentNews { get; }
    }
}
