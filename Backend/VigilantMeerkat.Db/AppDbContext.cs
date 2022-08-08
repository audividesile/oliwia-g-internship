using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VigilantMeerkat.Db.Model;
using VigilantMeerkat.Micro.Base.Config;

namespace VigilantMeerkat.Db
{
    public class AppDbContext : DbContext
    {
        public DbSet<ClientToken> Tokens { get; private set; }

        public DbSet<AdminConfig> AdminConfigs { get; private set; }

        public DbSet<ClientConfig> ClientConfigs { get; private set; }

        public DbSet<Log> Logs { get; private set; }

        public AppDbContext()
        {

        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ConfigHelper.ConnectionString[nameof(AppDbContext)]);
        }
    }
}
