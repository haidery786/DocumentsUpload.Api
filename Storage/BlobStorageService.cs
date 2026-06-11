using Azure.Storage.Blobs;
using DocumentsUpload.Api.DTOs;

namespace DocumentsUpload.Api.Storage
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["Azure:ConnectionString"];
            var containerName = configuration["Azure:ContainerName"];

            var client = new BlobServiceClient(connectionString);
            _container = client.GetBlobContainerClient(containerName);

            _container.CreateIfNotExists();
        }

        public async Task<List<FileMetaDataDto>> ListAsync()
        {
            List<FileMetaDataDto> files = new List<FileMetaDataDto>();

            await foreach (var file in _container.GetBlobsAsync()) 
            { 
                string uri = _container.Uri.ToString();
                var name = file.Name;
                var fullUri = $"{uri}/{name}";

                files.Add(new FileMetaDataDto
                {
                    BlobUrl = uri,
                    FileName = name,
                    ContentType = file.Properties.ContentType
                });
            }
            return files;
        }

        public async Task<ResponseFileMetaDataDto> UploadAsync(Stream fileStream, string fileName)
        {
            var response = new ResponseFileMetaDataDto();
            var blobClient = _container.GetBlobClient(fileName);

            await blobClient.UploadAsync(fileStream);

            response.Status = $"File {fileName} is uploaded successfully.";
            response.Error = false;
            response.FileDataDto.BlobUrl = blobClient.Uri.AbsoluteUri;
            response.FileDataDto.FileName = blobClient.Name;
            return response;
        }

        public async Task<FileMetaDataDto?> DownloadAsync(string blobFileName)
        {
            BlobClient file = _container.GetBlobClient(blobFileName);

            if (await file.ExistsAsync())
            {
                var data = await file.OpenReadAsync();
                Stream blobContent = data;
                var content = await file.DownloadContentAsync();

                string name = blobFileName;
                string contentType = content.Value.Details.ContentType;

                return new FileMetaDataDto
                {
                    Content = blobContent,
                    FileName = name,
                    ContentType = contentType
                };
            }

            return null;
        }

        public async Task<ResponseFileMetaDataDto> DeleteAsync(string fileName)
        {
            var file = _container.GetBlobClient(fileName);

            await file.DeleteAsync();

            return new ResponseFileMetaDataDto
            {
                Error = false,
                Status = $"File: {fileName} is deleted successfully."
            };
        }
    }
}
