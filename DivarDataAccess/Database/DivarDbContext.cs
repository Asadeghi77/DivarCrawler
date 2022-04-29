using DivarDataAccess.Database.Domain;
using Microsoft.EntityFrameworkCore;

namespace DivarDataAccess.Database
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DivarItem>().HasOne(c => c.Request)
                .WithMany(c => c.DivarItems)
                .HasForeignKey(c => c.RequestId)
                ;
        }
    }
}
