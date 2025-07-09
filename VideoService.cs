using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public class VideoService
    {
        private readonly IGridFSBucket _gridFSBucket;
        private readonly IMongoDatabase _database;
        private readonly string _videoContentType = "video/mp4"; // Укажите правильный content type

        public VideoService(IMongoClient client, string databaseName)
        {
            _database = client.GetDatabase(databaseName);
            _gridFSBucket = new GridFSBucket(_database);
        }

        public async Task<string> UploadVideoAsync(Stream stream, string filename)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new MongoDB.Bson.BsonDocument { { "contentType", _videoContentType } }
            };

            var videoId = await _gridFSBucket.UploadFromStreamAsync(filename, stream, options);
            return videoId.ToString();
        }

        public async Task<byte[]> DownloadVideoAsync(string filename)
        {
            var bytes = await _gridFSBucket.DownloadAsBytesByNameAsync(filename);
            return bytes;
        }

        public async Task<GridFSFileInfo> GetFileInfoAsync(string filename)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, filename);
            var findOptions = new GridFSFindOptions();
            using (var cursor = await _gridFSBucket.FindAsync(filter, findOptions))
            {
                return await cursor.FirstOrDefaultAsync();
            }
        }
    }
}
