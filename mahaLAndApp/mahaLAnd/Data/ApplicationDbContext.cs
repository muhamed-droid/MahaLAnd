using mahaLAnd.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace mahaLAnd.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //public DbSet<Employee> Employee { get; set; }
        //public DbSet<UserSupport> UserSupport { get; set; }
        //public DbSet<Administrator> Administrator { get; set; }
        //public DbSet<RegisteredUser> RegisteredUser { get; set; }
        //public DbSet<RegisteredUserPersonalProfile> RegisteredUserPersonalProfile { get; set; }
        //public DbSet<RegisteredUserProfessionalProfile> RegisteredUserProfessionalProfile { get; set; }
        //public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profile { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Statistics> Statistics { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Follow> Follow { get; set; }
        public DbSet<Request> Request { get; set; }
        public DbSet<Question> Question { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Employee>().ToTable("Employee");
            //modelBuilder.Entity<UserSupport>().ToTable("UserSupport");
            //modelBuilder.Entity<Administrator>().ToTable("Administrator");
            //modelBuilder.Entity<RegisteredUser>().ToTable("RegisteredUser");
            //modelBuilder.Entity<RegisteredUserPersonalProfile>().ToTable("RegisteredUserPersonalProfile");
            //modelBuilder.Entity<RegisteredUserProfessionalProfile>().ToTable("RegisteredUserProfessionalProfile");
            //modelBuilder.Entity<User>().ToTable("Profile");
            modelBuilder.Entity<Profile>().ToTable("Profile");
            modelBuilder.Entity<Post>().ToTable("Post");
            modelBuilder.Entity<Statistics>().ToTable("Statistics");
            modelBuilder.Entity<Notification>().ToTable("Notification");
            modelBuilder.Entity<Follow>().ToTable("Follow");
            modelBuilder.Entity<Request>().ToTable("Request");
            modelBuilder.Entity<Question>().ToTable("Question");
            base.OnModelCreating(modelBuilder);
        }
    }
}
