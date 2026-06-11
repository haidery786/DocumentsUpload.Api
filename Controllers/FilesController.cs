using DocumentsUpload.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocumentsUpload.Api.Controllers
{   
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }
        
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            var result = await _fileService.UploadAsync(formFile);

            return Ok(result);
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListAllBlobsAsync()
        {
            var files = await _fileService.ListAsync();

            if(files == null)
            {
                return NotFound();
            }

            return Ok(files);
        }

        [HttpGet("{fileName}/download")]
        public async Task<IActionResult> Download(string fileName)
        {
            var result = await _fileService.DownloadAsync(fileName);
            if (result == null || result.Content == null || string.IsNullOrEmpty(result.ContentType))
            {
                return NotFound();
            }
            return File(result.Content, result.ContentType, result.FileName);
        }

        [HttpDelete("fileName")]
        public async Task<IActionResult> Delete(string fileName)
        {
            var result = await _fileService.DeleteAsync(fileName);

            return Ok(result);
        }
    }
}
