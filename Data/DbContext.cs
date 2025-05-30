using Microsoft.EntityFrameworkCore;
using MotoMonitoramento.Models;

namespace MotoMonitoramento.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Moto> Motos { get; set; }
    }
}