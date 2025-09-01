using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Models
{
    public class WealthContext : DbContext
    {
        public WealthContext(DbContextOptions<WealthContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<Client> Clients { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Investment> Investments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Portfolio → Client (one-to-many)
            modelBuilder.Entity<Portfolio>()
                .HasOne(p => p.Client)                 
                .WithMany(c => c.Portfolios)           
                .HasForeignKey(p => p.ClientId)        
                .OnDelete(DeleteBehavior.Cascade);

            // Investment → Portfolio (one-to-many)
            modelBuilder.Entity<Investment>()
                .HasOne(i => i.Portfolio)              
                .WithMany(p => p.Investments)          
                .HasForeignKey(i => i.PortfolioId)     
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

    }
}
