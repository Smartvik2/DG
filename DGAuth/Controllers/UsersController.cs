using DGAuth.Data;
using DGAuth.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;

namespace DGAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _env;
        public UsersController(AppDbContext context, IWebHostEnvironment env)
        {
            _dbContext = context;
            _env = env;
        }


        [HttpPost("upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] UploadProfilePictureDto dto)
        {
            
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = userIdClaim;


            var user = await _dbContext.Users.FindAsync((userId));
            if (user == null) return Unauthorized();

            // Save image
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            if (dto.ProfilePicture == null || dto.ProfilePicture.Length == 0)
            {
                return BadRequest(new { message = "No file was uploaded." });
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ProfilePicture.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ProfilePicture.CopyToAsync(stream);
            }

            // Delete old one if exists
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                var oldPath = Path.Combine(_env.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            user.ProfilePictureUrl = $"/uploads/{fileName}";
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Profile picture uploaded.", imageUrl = user.ProfilePictureUrl });
        }
    }
}
