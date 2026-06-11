namespace DocumentsUpload.Api.DTOs
{
    public class ResponseFileMetaDataDto
    {
        public ResponseFileMetaDataDto()
        {
            FileDataDto = new FileMetaDataDto();
        }

        public string? Status { get; set; }

        public bool Error { get; set; }

        public FileMetaDataDto FileDataDto { get; set; }
    }
}
