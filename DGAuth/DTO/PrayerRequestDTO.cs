namespace DGAuth.DTO
{
    public class PrayerRequestDTO
    {
        public string PrayerRequest { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    }
}
