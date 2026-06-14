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
            var file = _container.GetBlobClient(fileName);

            try
            {
                if (await file.ExistsAsync())
                {
                    return new ResponseFileMetaDataDto
                    {
                        Status = $"File: {fileName} already exists.",
                        Error = true
                    };
                }

                await file.UploadAsync(fileStream);
                var properties = await file.GetPropertiesAsync();

                return new ResponseFileMetaDataDto
                {
                    Status = $"File '{fileName}' uploaded successfully.",
                    Error = false,
                    FileDataDto = new FileMetaDataDto
                    {
                        BlobUrl = file.Uri.AbsoluteUri,
                        FileName = file.Name,
                        ContentType = properties.Value.ContentType ?? "application/octete-stream"
                    }
                };
            }
            catch (Azure.RequestFailedException ex)
            {
                return new ResponseFileMetaDataDto
                {
                    Status = $"Azure error: {ex.Message}",
                    Error = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseFileMetaDataDto
                {
                    Status = $"Unexpected error: {ex.Message}",
                    Error = true
                };
            }
        }

        public async Task<FileMetaDataDto?> DownloadAsync(string blobFileName)
        {
            BlobClient file = _container.GetBlobClient(blobFileName);

            if (!await file.ExistsAsync())
            {
                throw new FileNotFoundException($"File: '{blobFileName}' not found.");
            }

            try
            {
                var stream = await file.OpenReadAsync();
                var properties = await file.GetPropertiesAsync();

                return new FileMetaDataDto
                {
                    Content = stream,
                    FileName = blobFileName,
                    ContentType = properties.Value.ContentType ?? "application/octet-stream"
                };
            }
            catch (Azure.RequestFailedException ex) 
            {
                throw new Exception("Azure download failed", ex);
            }    
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
