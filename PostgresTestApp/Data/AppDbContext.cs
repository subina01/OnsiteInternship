using System;
using System.Collections.Generic;
using System.Text;

namespace PostgresTestApp.Data
{
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=MyAppDb;Username=myuser;Password=mypassword");
        }
    }

}
