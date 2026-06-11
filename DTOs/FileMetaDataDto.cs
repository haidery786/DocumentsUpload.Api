namespace DocumentsUpload.Api.DTOs
{
    public class FileMetaDataDto
    {
        public string? BlobUrl { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }
    }
}
