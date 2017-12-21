using System.IO;
using Microsoft.EntityFrameworkCore;

namespace EmoteScavenger.Database
{
    public class StorageContext : DbContext
    {
        public DbSet<StorageItem> Items { get; set; }
        private FileInfo DatabaseFile { get; }

        public StorageContext(FileInfo dbf)
        {
            this.DatabaseFile = dbf;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($@"Filename=""{this.DatabaseFile.FullName}""");
        }
    }
}
