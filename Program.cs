using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MongoDBGridFSExample
{
    class Program
    {
        private const string ConnectionString = "mongodb://localhost:27017/";
        private const string DatabaseName = "praktikadb";
        private const string VideoFileName = "C:\\Users\\Student\\Videos\\praktika351.mp4";

        static async Task Main(string[] args)
        {
            var client = new MongoClient(ConnectionString);
            var database = client.GetDatabase(DatabaseName);
            var gridFSBucket = new GridFSBucket(database);

            try
            {
                // Загрузка видео
                using (var fileStream = File.OpenRead(VideoFileName))
                {
                    var options = new GridFSUploadOptions
                    {
                        Metadata = new MongoDB.Bson.BsonDocument { { "contentType", "video/mp4" } }  // Замените на правильный тип
                    };
                    var videoId = await gridFSBucket.UploadFromStreamAsync(VideoFileName, fileStream, options);
                    Console.WriteLine($"Video uploaded with ID: {videoId}");
                }


                // Получение информации о файле
                var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, VideoFileName);
                var fileInfo = await gridFSBucket.FindAsync(filter).Result.FirstOrDefaultAsync();

                if (fileInfo != null)
                {
                    Console.WriteLine($"Filename: {fileInfo.Filename}");
                    Console.WriteLine($"Size: {fileInfo.Length}");
                }

                // Скачивание видео
                string downloadedVideoFileName = "downloaded_video.mp4";
                using (var streamToDownloadTo = File.Create(downloadedVideoFileName))
                {
                    await gridFSBucket.DownloadToStreamByNameAsync(VideoFileName, streamToDownloadTo);
                    Console.WriteLine($"Video downloaded successfully to {downloadedVideoFileName}");
                }


            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error: {VideoFileName} not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
