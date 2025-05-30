﻿using MailKit.Net.Smtp;
using MimeKit;
using System.ComponentModel.DataAnnotations;
namespace DGAuth.Models
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string OtherNames { get; set; } = string.Empty;
        public string DepartmentInChurch { get; set; } = string.Empty;
        public string DepartmentInSchool { get; set; } = string.Empty;
        public string ResidentialAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
       
        

    }
}
