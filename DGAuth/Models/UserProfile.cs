using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DGAuth.Models
{
    public class UserProfile
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();
        [Required]
        public required string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual required User User { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string OtherNames { get; set; } = string.Empty;
        public string DepartmentInChurch { get; set; } = string.Empty;
        public string DepartmentInSchool { get; set; } = string.Empty;
        public string ResidentialAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
