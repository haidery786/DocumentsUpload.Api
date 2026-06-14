namespace DocumentsUpload.Api.DTOs
{
    public record ResponseFileMetaDataDto
    {
        public FileMetaDataDto FileDataDto { get; init; } = new();
        public string? Status { get; init; }
        public bool Error { get; init; }
    }
}
