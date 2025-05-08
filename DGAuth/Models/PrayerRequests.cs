using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace DGAuth.Models
{
    public class PrayerRequests
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PrayerRequest { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public required string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

    }
}
