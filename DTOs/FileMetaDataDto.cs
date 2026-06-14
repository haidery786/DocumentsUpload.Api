namespace DocumentsUpload.Api.DTOs
{
    public record FileMetaDataDto
    {
        public string? BlobUrl { get; init; }
        public string? FileName { get; init; }
        public string? ContentType { get; init; }
        public Stream? Content { get; init; }
    }
}
