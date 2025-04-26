namespace DGAuth.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        //public string FirstName { get; set; } = string.Empty;
        public string OtherNames { get; set; } = string.Empty;
        public string DepartmentInChurch { get; set; } = string.Empty;
        public string DepartmentInSchool { get; set; } = string.Empty;
        public string ResidentialAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
