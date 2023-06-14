using DiscordBotSyriaRP.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordBotSyriaRP.EF
{
    public class KPKContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public KPKContext()
        {
            //Database.EnsureCreated();
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite("Data Source=KPK.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}