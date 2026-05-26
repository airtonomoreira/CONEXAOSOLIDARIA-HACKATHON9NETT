using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace DonationService;

public class DonationDbContext : DbContext
{
    public DonationDbContext(DbContextOptions<DonationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Doacao> Doacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Valor).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Pendente");
            entity.Property(e => e.DataDoacao).HasDefaultValueSql("GETDATE()");
        });
    }
}
