using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using WebApi.Services;  
using System;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideosController : ControllerBase
    {
        private readonly VideoService _videoService;
        private readonly IContentTypeProvider _contentTypeProvider;

        public VideosController(VideoService videoService, IContentTypeProvider contentTypeProvider)
        {
            _videoService = videoService;
            _contentTypeProvider = contentTypeProvider;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded or file is empty.");
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    string videoId = await _videoService.UploadVideoAsync(stream, file.FileName);
                    return Ok(new { VideoId = videoId, Filename = file.FileName });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("download/{filename}")]
        public async Task<IActionResult> DownloadVideo(string filename)
        {
            try
            {
                var fileBytes = await _videoService.DownloadVideoAsync(filename);
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return NotFound($"Video with filename '{filename}' not found.");
                }

                if (!_contentTypeProvider.TryGetContentType(filename, out var contentType))
                {
                    contentType = "application/octet-stream"; 
                }

                return File(fileBytes, contentType, filename);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("info/{filename}")]
        public async Task<IActionResult> GetVideoInfo(string filename)
        {
            try
            {
                var fileInfo = await _videoService.GetFileInfoAsync(filename);
                if (fileInfo == null)
                {
                    return NotFound($"Video with filename '{filename}' not found.");
                }

                return Ok(new
                {
                    Filename = fileInfo.Name, 
                    Length = fileInfo.Length,
                    UploadDate = fileInfo.CreationTime 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
