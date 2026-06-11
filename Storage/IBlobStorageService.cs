using DocumentsUpload.Api.DTOs;

namespace DocumentsUpload.Api.Storage
{
    public interface IBlobStorageService
    {
        Task<ResponseFileMetaDataDto> UploadAsync(Stream fileStream, string fileName);
        Task<FileMetaDataDto?> DownloadAsync(string blobFileName);
        Task<List<FileMetaDataDto>> ListAsync();

        Task<ResponseFileMetaDataDto> DeleteAsync(string fileName);
    }
}