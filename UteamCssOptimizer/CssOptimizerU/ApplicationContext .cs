using CssOptimizerU.DM;
using Microsoft.EntityFrameworkCore;



namespace CssOptimizerU
{
    public class CssAnalyzerContext : DbContext
    {


        public DbSet<File> Files { get; set; }
        public DbSet<Selector> Selector { get; set; }
        public DbSet<Usage> Usages { get; set; }


        private readonly string _connectionString;

        public CssAnalyzerContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>()
            .HasOne(b => b.Selector)
            .WithOne(i => i.File);

            modelBuilder.Entity<Usage>()
           .HasOne(b => b.Selector)
           .WithOne(i => i.Usage);

        }
    }
}
