using Microsoft.EntityFrameworkCore;

namespace SimulacionBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<SimulacionRecord> Simulaciones { get; set; }
    }
}