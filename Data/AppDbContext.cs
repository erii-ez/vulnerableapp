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

            // IMPORTANTE: Sustituye estos strings por hashes reales de Bcrypt 
            // generados previamente para "admin", "123456" y "password".
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "$2b$12$4ViAVOygLkUqH7zS88Va8eqnYT8SNNSLUWsUEtDiqtpAVY0anO9Aq",
                    Email = "admin@test.com",
                    Balance = 1000m,
                    CreatedAt = fechaEstatica
                },
                new User
                {
                    Id = 2,
                    Username = "user1",
                    PasswordHash = "$2b$12$8kUykxelX5lI8iQ/DP1oTeuK/1sWesogHN.TVZR.hETNNDUslBVTW",
                    Email = "user@test.com",
                    Balance = 500m,
                    CreatedAt = fechaEstatica
                },
                new User
                {
                    Id = 3,
                    Username = "user2",
                    PasswordHash = "$$2b$12$fDaWV7Cx6ON8GPx4Pdb8Q.tzCVtu/xxl4j7RHQF20HLviyeGL/V26",
                    Email = "user2@test.com",
                    Balance = 750m,
                    CreatedAt = fechaEstatica
                }
            );
        }
    }
}