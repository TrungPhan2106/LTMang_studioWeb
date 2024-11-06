using StudioManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;

namespace StudioManagement.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        public DbSet<User>? Users { get; set; }
        public DbSet<Member>? Members { get; set; }
        public DbSet<Studio>? Studios { get; set; }
        public DbSet<Employees>? Employees { get; set; }
        public DbSet<Role>? Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Member>(e =>
            {
                e.Property(o => o.Gender)
                    .HasColumnType("bit");
            });
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();  // Đảm bảo email là duy nhất trong cơ sở dữ liệu

            // Cấu hình mối quan hệ giữa User và Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.User)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.SetNull);  // Xóa cascade cho Role nếu cần thiết

            // Cấu hình mối quan hệ giữa Member và Studio
            modelBuilder.Entity<Member>()
                .HasOne(m => m.Studio)
                .WithMany()
                .HasForeignKey(m => m.StudioID)
                .OnDelete(DeleteBehavior.SetNull); // Khi Member bị xóa, Studio ID vẫn tồn tại

            // Cấu hình mối quan hệ giữa Employee và Studio
            modelBuilder.Entity<Employees>()
                .HasOne(e => e.Studio)
                .WithMany()
                .HasForeignKey(e => e.StudioID)
                .OnDelete(DeleteBehavior.SetNull);  // Khi Employee bị xóa, Studio ID vẫn tồn tại

            // Cấu hình mối quan hệ giữa Employee và User
            modelBuilder.Entity<Employees>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employees)
                .HasForeignKey<Employees>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa cascade khi User bị xóa

            // Cấu hình mối quan hệ giữa User và Member
            modelBuilder.Entity<Member>()
                .HasOne(m => m.User)
                .WithOne(u => u.Member)
                .HasForeignKey<Member>(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình mối quan hệ giữa Role và User
            modelBuilder.Entity<Role>()
                .HasMany(r => r.User)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
