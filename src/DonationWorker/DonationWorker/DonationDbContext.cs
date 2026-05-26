using Microsoft.EntityFrameworkCore;

namespace DonationWorker;

public class DonationDbContext : DbContext
{
    public DonationDbContext(DbContextOptions<DonationDbContext> options)
        : base(options)
    {
    }
}
