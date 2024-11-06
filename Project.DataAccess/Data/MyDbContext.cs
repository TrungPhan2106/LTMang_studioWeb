using MemberManagement.Data;
using MemberManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;

namespace MemberManagement.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        public DbSet<Member> Member { get; set; }
        public DbSet<Studio> Studio { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Member>(e =>
            {
                e.Property(o => o.Gender).HasConversion(new BoolToZeroOneConverter<Int16>())
                    .HasColumnType("bit");

                e.Property(o => o.IsDeleted).HasConversion(new BoolToZeroOneConverter<Int16>())
                    .HasColumnType("bit");
            });
        }
    }
}
