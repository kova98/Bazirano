using Bazirano.Models.Board;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public interface IWriter
    {
        /// <summary>
        /// Uploads an image and returns the file name.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>The name of the file</returns>
        Task<string> UploadImage(IFormFile file);

        /// <summary>
        /// Downloads an image from the given url and returns the file name.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns>The name of the file</returns>
        Task<string> DownloadImageFromUrl(string imageUrl);

        double ByteToMegabyte(long bytes);

        void DeleteImage(string image);

        string GetSampleColumnText();
    }
}