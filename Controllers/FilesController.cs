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

            if(result.Error)
                return Conflict(result.Error); // 409 for duplicate error

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

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> Download(string fileName)
        {
            try
            {
                var file = await _fileService.DownloadAsync(fileName);

                return File(file.Content, file.ContentType, file.FileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("fileName")]
        public async Task<IActionResult> Delete(string fileName)
        {
            var result = await _fileService.DeleteAsync(fileName);

            return Ok(result);
        }
    }
}
