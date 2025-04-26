﻿namespace DGAuth.Models
{
    public class ResetPasswordRequest
    { 
        
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        // Simulated token
        public string NewPassword { get; set; } = string.Empty;

    }
}
