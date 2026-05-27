using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProyectoITV.Entity;
using ProyectoITV.Report;
using ProyectoITV.Repositories;
using ProyectoITV.Services;
using Serilog;

namespace ProyectoITV.Infraestructure;

/// <summary>
/// Clase estática para la configuración de los servicios y dependencias necesarias para el funcionamiento del programa
/// </summary>
public static class DependenciesProvider
{
    /// <param name="services">Los servicios del programa</param>
    /// <param name="provider">Nombre del proveedor a utilizar</param>
    /// <param name="connectionString">String de conexión a la base de datos</param>
    /// <returns>El contenedor de servicios con las dependencias registradas.</returns>
    public static IServiceCollection AddDependenciesProvider(this IServiceCollection services, string provider,
        string connectionString)
    {
        switch (provider)
        {
            // Registra el DbContext de EFCore y su repositorio
            case "EFCore":
                services.AddDbContext<AppDbContext>(o => o.UseSqlite(connectionString));
                services.AddScoped<ICitaRepository, EfCoreCitaRepository>();
                break;
            // Registra la conexión genérica para Dapper y su repositorio
            case "Dapper":
                services.AddScoped<IDbConnection>(c => new SqliteConnection(connectionString));
                services.AddScoped<ICitaRepository, DapperCitaRepository>();
                break;
            // Registra la conexión Sqlite para ADO y su repositorio
            case "ADO":
                services.AddScoped<SqliteConnection>(c => new SqliteConnection(connectionString));
                services.AddScoped<ICitaRepository, AdoCitaRepository>();
                break;
            // Uso de Dapper por defecto
            default:
                services.AddScoped<IDbConnection>(c => new SqliteConnection(connectionString));
                services.AddScoped<ICitaRepository, DapperCitaRepository>();
                break;
        }
        
        // Registro del servicio comun para todas las opciones
        services.AddScoped<CitaService>();
        // Registro y configuracion del logging
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        // Registro del service de informes
        services.AddScoped<IInformeService, InformeService>();
        return services;
    }
}