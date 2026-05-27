using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProyectoITV.Front;
using ProyectoITV.Infraestructure;
using Serilog;

namespace ProyectoITV.Front;

/// <summary>
/// Clase de entrada de la aplicación
/// Gestiona la configuración del host, la DI y el sistema de logs
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    /// <summary>
    /// Se ejecuta cuando la aplicación inicia
    /// Configura gran parte de la app
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        // Configuración del sistema de logs
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/itv-log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 5)
            .CreateLogger();

        try
        {
            // Crea el Host
            var builder = Host.CreateApplicationBuilder();
            builder.Logging.AddSerilog();
            
            // Lee la configuración de appsettings.json (menos borrado lógico)
            var config = builder.Configuration;
            string provider = config["Database:Provider"] ?? "Dapper";
            string connectionString = config["Database:ConnectionString"] ?? "Data Source=ITV.db";
            
            // Llama a la clase dependendencies provider
            builder.Services.AddDependenciesProvider(provider, connectionString);
            // Cada que se pida un viewmodel se le creará uno
            builder.Services.AddTransient<MainViewModel>();
            // Solo una instancia de mainwindow
            builder.Services.AddSingleton<MainWindow>();

            _host = builder.Build();
            
            // Inicializa la vista principal consiguiendo el viewmodel
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            
            Log.Information("Aplicación iniciada");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error al iniciar la aplicación");
            MessageBox.Show("Error al iniciar");
            Shutdown();
        }
    }

    /// <summary>
    /// Se ejecuta al cerrar la aplicación, cierra los logs y escribe todos los pendientes
    /// </summary>
    protected override async void OnExit(ExitEventArgs e)
    {
        Log.Information("Aplicacion cerrandose");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}