using DivarCrawler.Database.Domain;
using Microsoft.EntityFrameworkCore;

namespace DivarCrawler.Database
{
    public class DivarDbContext : DbContext
    {
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<DivarItem> DivarItems { get; set; }

        public string DbPath { get; private set; }

        public DivarDbContext()
        {
            var path = Environment.CurrentDirectory;
            DbPath = System.IO.Path.Join(path, "DivarDb.db");
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }
    }
}
