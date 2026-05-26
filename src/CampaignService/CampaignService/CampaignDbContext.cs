using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace CampaignService;

public class CampaignDbContext : DbContext
{
    public CampaignDbContext(DbContextOptions<CampaignDbContext> options)
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
            entity.Property(e => e.ValorArrecadado).HasPrecision(18, 2).HasDefaultValue(0);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Ativa");
        });
    }
}
