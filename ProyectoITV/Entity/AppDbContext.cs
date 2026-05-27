using ProyectoITV.Models;
namespace ProyectoITV.Entity;
using Microsoft.EntityFrameworkCore;
 
/// <summary>
/// Representa el contexto de la base de datos
/// Sirve para conectar las clases a la base de datos
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Representa la tabla Citas en la base de datos.
    /// </summary>
    public DbSet<Cita> Citas { get; set; }
    
    /// <summary>
    /// Constructor que permite inyectar opciones de configuración
    /// </summary>
    /// <param name="options">Opciones de configuración del DbContext</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Configura el comportamiento de la base de datos. Se ejecuta automáticamente
    /// </summary>
    /// <param name="optionsBuilder">Sirve para crear las opciones de conexión</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Comprueba si la configuración ya ha sido establecida
        if (!optionsBuilder.IsConfigured)
        {
            // Si no está configurado, se establece la conexión por defecto a un archivo SQLite
            optionsBuilder.UseSqlite("Data Source=ITV.db");
        }
    }
}