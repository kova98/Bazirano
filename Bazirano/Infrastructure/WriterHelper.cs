using Bazirano.Models.Board;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text.RegularExpressions;
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
            string cleanFileName = Regex.Replace(fileName, @"\s+", "");

            string filePath = Path.Combine(Directory.GetCurrentDirectory(),
                                           "wwwroot", "images",
                                           cleanFileName);
            post.Image = cleanFileName;

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
