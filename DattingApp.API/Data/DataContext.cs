using System.Diagnostics.CodeAnalysis;
using DattingApp.API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DattingApp.API.Data
{
    public class DataContext : IdentityDbContext // : DbContext //without Identity
        //If Id is int, then we need to do this to ease up the migration process
        <User, Role, int,
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, 
        IdentityRoleClaim<int>, IdentityUserToken<int>>  
    {
        public DataContext([NotNullAttribute] DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Value> Values { get; set; }
        // public DbSet<User> Users { get; set; } // Identity Takes care of this
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //To work with Identity
            base.OnModelCreating(builder);
            //Specify the identity relations
            builder.Entity<UserRole>(userRole => 
            {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();                    
            });
            // No identity
            builder.Entity<Like>()
                .HasKey(k => new { k.LikerId, k.LikeeId });

            builder.Entity<Like>()
                .HasOne(u => u.Likee)
                .WithMany(u => u.Likers)
                .HasForeignKey(u => u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(u => u.Liker)
                .WithMany(u => u.Likees)
                .HasForeignKey(u => u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}