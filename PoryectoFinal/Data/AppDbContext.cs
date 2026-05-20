using Microsoft.EntityFrameworkCore;
using PoryectoFinal.Models;

namespace PoryectoFinal.Data;

public class AppDbContext : DbContext
{
    public DbSet<Cita> Citas { get; set; }
    public DbSet<Vehiculo> Vehiculos { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=ITV.db");
        }
    }
}