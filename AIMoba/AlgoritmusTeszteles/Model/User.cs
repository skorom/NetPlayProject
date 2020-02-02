using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AlgorithmTestingConsoleApp.Model
{
    public class UsersContext : DbContext
    {
        public UsersContext()
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(
                @"Data Source=users.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
        }
    }
    [Table("Users")]
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public int score { get; set; }

        public override string ToString()
        {
            return "id: " + id + ", name: " + name + ", password: " + password + ", score: " + score;
        }

    }
}
