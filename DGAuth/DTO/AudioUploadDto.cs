namespace DGAuth.DTO
{
    public class AudioUploadDto
    {
        public required IFormFile File { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime Date { get; set; } 
    }
}
