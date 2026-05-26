using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace TransparencyService;

public class TransparencyDbContext : DbContext
{
    public TransparencyDbContext(DbContextOptions<TransparencyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Campanha> Campanhas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campanha>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).IsRequired();
            entity.Property(e => e.MetaFinanceira).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.ValorArrecadado).HasPrecision(18, 2);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
        });
    }
}
