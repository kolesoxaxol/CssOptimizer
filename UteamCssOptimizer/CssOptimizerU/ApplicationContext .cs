using CssOptimizerU.DM;
using Microsoft.EntityFrameworkCore;

namespace CssOptimizerU
{
    public class CssAnalyzerContext : DbContext
    {
        public DbSet<File> Files { get; set; }
        public DbSet<Selector> Selector { get; set; }
        public DbSet<Usage> Usages { get; set; }

        public CssAnalyzerContext(DbContextOptions<CssAnalyzerContext> options) : base(options)
        {
            
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>()
            .HasMany(b => b.Selectors)
            .WithOne(i => i.File);

            modelBuilder.Entity<File>()
            .HasMany(b => b.Usages)
            .WithOne(i => i.File);

            modelBuilder.Entity<Selector>()
           .HasOne(b => b.Usage)
           .WithOne(i => i.Selector)
           .HasForeignKey<Usage>(p => p.SelectorId);

        }
    }
}
