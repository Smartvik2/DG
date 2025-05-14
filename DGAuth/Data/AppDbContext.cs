using Microsoft.EntityFrameworkCore;
using DGAuth.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using static DGAuth.Models.LSTSFORM;


namespace DGAuth.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        //public DbSet<User> Users { get; set; }
        
        //public DbSet<LSTSRegistration> LSTSRegistrations { get; set; }

        public DbSet<UserProfile> Profiles { get; set; }
        public DbSet<LSTSFORM> LstsForms { get; set; }
        public DbSet<PrayerRequests> Requests { get; set; }
        public DbSet<AudioMessage> AudioMessages { get; set; }
        
        public DbSet<Announcement> Announcements { get; set; }
        //public DbSet<ProfilePicture> 
        




        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(propa => propa.User)
                .HasForeignKey<UserProfile>(p => p.UserId);
        }






    }
}
