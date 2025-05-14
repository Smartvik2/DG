namespace DGAuth.DTO
{
    public class AnnouncementsDTO
    {
        //public string Announcements { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
}
