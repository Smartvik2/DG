namespace DGAuth.DTO
{
    public class LSTSFORMDTO
    {
        public string Surname { get; set; } = string.Empty;
        public string OtherNames { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ResidentialAddress { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DepartmentInChurch { get; set; } = string.Empty;
        public string PositionInChurch { get; set; } = string.Empty;
        public string DepartmentInSchool { get; set; } = string.Empty;
        public int Level { get; set; }
        public string Student { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
