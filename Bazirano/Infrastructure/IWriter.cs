using Bazirano.Models.Board;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public interface IWriter
    {
        Task UploadImage(BoardPost post, IFormFile file);

        double ByteToMegabyte(long bytes);

        void DeleteImage(string image);

        string GetSampleColumnText();
    }
}