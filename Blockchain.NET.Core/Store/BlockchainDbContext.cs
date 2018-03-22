using Blockchain.NET.Core;
using Blockchain.NET.Core.Mining;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Core.Store
{
    public class BlockchainDbContext : DbContext
    {
        public static void InitializeMigrations()
        {
            var db = new BlockchainDbContext();
            db.Database.EnsureCreated();
            db.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=blockchain.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public virtual DbSet<Block> Blocks { get; set; }

        public virtual DbSet<Transaction> Transactions { get; set; }

        public virtual DbSet<Input> Inputs { get; set; }

        public virtual DbSet<Output> Outputs { get; set; }
    }
}
