using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApi.DataAccessLayer.Models;

namespace WebApi.DataAccessLayer
{
    public class DB : DbContext
    {
        public DB(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivitySolution> ActivitySolutions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public IQueryable<User> Students => Users.Where(u => u.Role == UserRole.Worker);

        public IQueryable<User> VisibleUsers => Users.Where(u => u.Role != UserRole.Superadmin);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Group>().HasMany(g => g.Users).WithMany(u => u.Groups);
            modelBuilder.Entity<Group>().HasOne(g => g.Leader);
            /*
            modelBuilder.Entity<Company>().HasOne(c => c.Owner).WithOne(u => u.Company).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<UserNotification>().HasKey(un => new { un.UserId, un.NotificationId });
            modelBuilder.Entity<JobApplication>(entity => entity.Property(ja => ja.Priority).HasDefaultValue(JobApplicationPriority.Undecided));*/
        }
    }
}
