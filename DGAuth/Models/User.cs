using Microsoft.AspNetCore.Identity;


namespace DGAuth.Models

{
    public class User : IdentityUser
    {
        
        public virtual  UserProfile? Profile { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string OtherNames { get; set; } = string.Empty;
        public string DepartmentInChurch { get; set; } = string.Empty;
        public string DepartmentInSchool { get; set; } = string.Empty;
        public string ResidentialAddress { get; set; } = string.Empty;
        //public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }

        public string Role { get; set; } = string.Empty;
    }
}
