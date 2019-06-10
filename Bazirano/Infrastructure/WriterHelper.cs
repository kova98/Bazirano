using Bazirano.Models.Board;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public class WriterHelper
    {
        public static bool IsImageFile(IFormFile file)
        {
            return file.ContentType.StartsWith("image");
        }

        public static async Task UploadImage(BoardPost post, IFormFile file)
        {
            Random rnd = new Random();
            string fileName = rnd.Next(10000, 99999).ToString() + file.FileName;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(),
                                           "wwwroot\\images",
                                           fileName);
            post.Image = fileName;

            if (IsImageFile(file))
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
        }

        public static double ByteToMegabyte(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}
