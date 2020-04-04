using Bazirano.Models.Board;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public class WriterHelper : IWriter
    {
        public string GetSampleColumnText()
        {
            return File.ReadAllText("sample-article.md");
        }

        public async Task UploadImage(BoardPost post, IFormFile file)
        {
            Random rnd = new Random();
            string fileName = rnd.Next(10000, 99999).ToString() + file.FileName;
            string cleanFileName = Regex.Replace(fileName, @"\s+", "");
            string filePath = GetFullPath(cleanFileName);

            post.Image = cleanFileName;

            if (IsImageFile(file))
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
        }
        private bool IsImageFile(IFormFile file)
        {
            return file.ContentType.StartsWith("image");
        }

        public void DeleteImage (string image)
        {
            if (!string.IsNullOrEmpty(image))
            {
                var path = GetFullPath(image);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        public double ByteToMegabyte(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        private static string GetFullPath(string fileName)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
        }
    }
}
