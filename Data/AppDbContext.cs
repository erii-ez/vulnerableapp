using System;
using Microsoft.EntityFrameworkCore;
using VulnerableApp.Models;

namespace VulnerableApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Balance)
                .HasColumnType("decimal(18,2)");
            DateTime fechaEstatica = new DateTime(2026, 1, 1);
            modelBuilder.Entity<User>().HasData(
                new User { 
                    Id = 1, 
                    Username = "admin", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"), 
                    Email = "admin@test.com", 
                    Balance = 1000m, 
                    CreatedAt = fechaEstatica 
                },
                new User { 
                    Id = 2, 
                    Username = "user1", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), 
                    Email = "user@test.com", 
                    Balance = 500m, 
                    CreatedAt = fechaEstatica 
                },
                new User { 
                    Id = 3, 
                    Username = "user2", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), 
                    Email = "user2@test.com", 
                    Balance = 750m, 
                    CreatedAt = fechaEstatica 
                }
            );
        }
    }
}