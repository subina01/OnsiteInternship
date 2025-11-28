using Microsoft.EntityFrameworkCore;
using sample_redistestapp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace sample_redistestapp.Data
{

    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // SQL Server connection string
            optionsBuilder.UseSqlServer("Server=DESKTOP-NP9TQ4B\\SQLEXPRESS;Database=User;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;");
        }
    }
}
