using AuthApi.Entities;
using AuthApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AuthApi.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        private readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(Configuration.GetConnectionString("AuthApiDatabase"));
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>().HasData(
        //    new { BlogId = 1, PostId = 2, Title = "Second post", Content = "Test 2" });
        //}
    }
}
