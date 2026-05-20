using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PoryectoFinal.Interfaces;
using PoryectoFinal.Repositories;
using PoryectoFinal.Services;

namespace PoryectoFinal.Data;

public static class DependenciesProvider
{
    public static IServiceCollection AddDependenciesProvider(this IServiceCollection services, string provider,
        string connectionString)
    {
        switch (provider)
        {
            case "EFCore":
            services.AddDbContext<AppDbContext>(o => o.UseSqlite(connectionString));
            services.AddScoped<ICitaRepository, EfCoreCitaRepository>();
            services.AddScoped<IVehiculoRepository, EfCoreVehiculoRepository>();
            break;
            case "Dapper":
                services.AddScoped<IDbConnection>(c => new SqliteConnection(connectionString));
                services.AddScoped<ICitaRepository, DapperCitaRepository>();
                services.AddScoped<IVehiculoRepository, DapperVehiculoRepository>();
                break;
            case "ADO":
                services.AddScoped<IDbConnection>(c => new SqliteConnection(connectionString));
                services.AddScoped<ICitaRepository, AdoCitaRepository>();
                services.AddScoped<IVehiculoRepository, AdoVehiculoRepository>();
                break;
            default:
                services.AddScoped<IDbConnection>(c => new SqliteConnection(connectionString));
                services.AddScoped<ICitaRepository, DapperCitaRepository>();
                services.AddScoped<IVehiculoRepository, DapperVehiculoRepository>();
                break;
        }
        
        services.AddScoped<CitaService>();
        return services;
    }
}