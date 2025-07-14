using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public class VideoService
    {
        private readonly string _videoUploadPath;
        private readonly IConfiguration _configuration;

        public VideoService(IConfiguration configuration)  // IMongoClient больше не нужен, можно убрать
        {
            _configuration = configuration;
            _videoUploadPath = configuration["VideoUploadPath"];
        }

        public async Task<string> UploadVideoAsync(Stream stream, string fileName)
        {
            string filePath = Path.Combine(_videoUploadPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            return fileName; // Возвращаем имя файла как ID (для простоты)
        }

        public async Task<byte[]> DownloadVideoAsync(string filename)
        {
            string filePath = Path.Combine(_videoUploadPath, filename);

            if (!File.Exists(filePath))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(filePath);
        }

        public async Task<System.IO.FileInfo> GetFileInfoAsync(string filename)
        {
            string filePath = Path.Combine(_videoUploadPath, filename);

            if (!File.Exists(filePath))
            {
                return null;
            }

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
            return fileInfo; // Возвращаем System.IO.FileInfo напрямую
        }

    }
}
