using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace DGAuth.Models
{
    public class LSTSFORM
    {
       
        
            public string Surname { get; set; } = string.Empty;
            public string OtherNames { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string ResidentialAddress { get; set; } = string.Empty;
            [Key]
            public string Email { get; set; } = string.Empty;
            public string DepartmentInChurch { get; set; } = string.Empty;
            public string PositionInChurch { get; set; } = string.Empty;
            public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        

    }
}
