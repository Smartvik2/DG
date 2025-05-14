using System.ComponentModel.DataAnnotations;

namespace DGAuth.Models
{
    public class AudioMessage
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Sunday|Midweek|Friday", ErrorMessage = "Category must be Sunday, Midweek, or Friday")]
        public string Category { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string FilePath { get; set; } = string.Empty;
    }
}
