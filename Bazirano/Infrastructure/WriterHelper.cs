using Bazirano.Models.Board;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public class WriterHelper : IWriter
    {
        private readonly ILogger<WriterHelper> logger;

        public WriterHelper(ILogger<WriterHelper> logger)
        {
            this.logger = logger;
        }

        public string GetSampleColumnText()
        {
            return File.ReadAllText("sample-article.md");
        }

        public async Task<string> UploadImage(IFormFile upload)
        {
            var extension = Path.GetExtension(upload.FileName);
            var fileName = GetFileName(extension);
            var filePath = GetFullPath(fileName);

            if (IsImageFile(upload))
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }
            }

            return fileName;
        }

        public async Task<string> DownloadImageFromUrl(string imageUrl)
        {
            string fileName = GetFileName(".png");
            var filePath = GetFullPath(fileName);

            using WebClient webClient = new WebClient();

            var data = await webClient.DownloadDataTaskAsync(new Uri(imageUrl));

            using MemoryStream memoryStream = new MemoryStream(data);

            try
            {
                using var image = Image.FromStream(memoryStream);
                image.Save(filePath, ImageFormat.Png);
            }
            catch (ArgumentException)
            {
                logger.LogError("Non-image url provided as board image source: " + imageUrl);
                return null;
            }

            return fileName;
        }

        private static string GetFileName(string extension)
        {
            return Guid.NewGuid() + extension;
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
