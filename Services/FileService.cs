using DocumentsUpload.Api.DTOs;
using DocumentsUpload.Api.Storage;

namespace DocumentsUpload.Api.Services
{
    public class FileService : IFileService
    {
        private readonly IBlobStorageService _blobStorageService;

        public FileService(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        public async Task<ResponseFileMetaDataDto> UploadAsync(IFormFile file)
        {
            var fileName = file.FileName;
            using var stream = file.OpenReadStream();
            var response = await _blobStorageService.UploadAsync(stream, fileName);

            return response;
        }

        public async Task<List<FileMetaDataDto>> ListAsync()
        {
            return await _blobStorageService.ListAsync();
        }

        public async Task<FileMetaDataDto?> DownloadAsync(string fileName)
        {
            return await _blobStorageService.DownloadAsync(fileName);
        }

        public async Task<ResponseFileMetaDataDto> DeleteAsync(string fileName)
        {
            return await _blobStorageService.DeleteAsync(fileName);
        }
    }
}
