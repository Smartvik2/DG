using Microsoft.AspNetCore.Mvc;
using DGAuth.Data;
using DGAuth.DTO;
using DGAuth.Migrations;
using DGAuth.Models;
using DGAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DGAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AudioMessageController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public AudioMessageController(AppDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Upload([FromForm] AudioUploadDto model)
           
        {
            var allowedCategories = new[] { "Sunday", "Midweek", "Friday" };
            if (!allowedCategories.Contains(model.Category))
                return BadRequest("Category must be Sunday, Midweek, or Friday.");

            if (model.File == null || model.File.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "audio");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{model.File.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            var audioMessage = new AudioMessage
            {
                Title = model.Title,
                Category = model.Category,
                Date = model.Date,
                FilePath = $"audio/{fileName}"
            };

            _dbContext.AudioMessages.Add(audioMessage);
            await _dbContext.SaveChangesAsync();

           

            return Ok(audioMessage);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] string? category = null)
        {
            var query = _dbContext.AudioMessages.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(a => a.Category == category);

            var messages = await query.OrderByDescending(a => a.Date).ToListAsync();
            return Ok(messages);
        }

        [HttpGet("download/{id}")]
        [Authorize]
        public async Task<IActionResult> Download(int id)
        {
            var message = await _dbContext.AudioMessages.FindAsync(id);
            if (message == null)
                return NotFound();

            var path = Path.Combine(_env.WebRootPath, message.FilePath);
            if (!System.IO.File.Exists(path))
                return NotFound("Audio file not found.");

            var bytes = await System.IO.File.ReadAllBytesAsync(path);
            return File(bytes, "audio/mpeg", Path.GetFileName(path));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _dbContext.AudioMessages.FindAsync(id);
            if (message == null)
                return NotFound();

            var path = Path.Combine(_env.WebRootPath, message.FilePath);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _dbContext.AudioMessages.Remove(message);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

    }
}
