using DocumentsUpload.Api.DTOs;

namespace DocumentsUpload.Api.Services
{
    public interface IFileService
    {
        Task<ResponseFileMetaDataDto> UploadAsync(IFormFile file);
        Task<FileMetaDataDto?> DownloadAsync(string fileName);
        Task<List<FileMetaDataDto>> ListAsync();
        Task<ResponseFileMetaDataDto> DeleteAsync(string fileName);
    }
}