using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>                     // we just indicate that we want an INT id
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        // This one is already present in the inherited class:
        // public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

        // for Likes Many-to-Many relationship to work:
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);          // explanation of base: https://stackoverflow.com/questions/2644316/what-really-is-the-purpose-of-base-keyword-in-c

            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            
            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            
            builder.Entity<UserLike>()
                .HasKey(k => new {k.SourceUserId, k.TargetUserId});
            
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)              // who likes        1
                .WithMany(l => l.LikedUsers)            // whom             N           [where to store whom user likes]
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);          // read comment below
            
            builder.Entity<UserLike>()
                .HasOne(s => s.TargetUser)              // who is liked     1
                .WithMany(l => l.LikedByUsers)          // by whom          N           [where to store by whom user is liked]
                .HasForeignKey(s => s.TargetUserId)
                .OnDelete(DeleteBehavior.Cascade);          // if we had used SQL Server and not SQLite/PostgreSQL (Postgres later on when publishing), then "Cascade" should have been replaced by "NoAction" (could also be done in the statement above rather that this one). Again, this is only applied in case we use SQL Server, and nothing else apart from it.
            
            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);         // if user deletes his account, his message should NOT be deleted
            
            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}