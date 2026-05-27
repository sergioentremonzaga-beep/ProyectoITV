using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProyectoITV.Enums;
using ProyectoITV.Infraestructure;
using ProyectoITV.Models;
using ProyectoITV.Report;
using ProyectoITV.Services;
using ProyectoITV.Validators;
using Serilog;

namespace ProyectoITV.Front;

/// <summary>
/// ViewModel principal que gestiona la lógica de la vista principal de la app
/// Utiliza el patrón MVVM
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly CitaService _citaService;
    private readonly bool _borradoLogico;
    private readonly ILogger<MainViewModel> _logger;
    private readonly IInformeService _informeService;
    
    // Propiedades para Crear
    [ObservableProperty] private string _dni = string.Empty;
    [ObservableProperty] private string _matricula = string.Empty;
    [ObservableProperty] private string _marca = string.Empty;
    [ObservableProperty] private string _modelo = string.Empty;
    [ObservableProperty] private TipoMotor _motor = TipoMotor.Gasolina;
    [ObservableProperty] private DateTime _fechaInspeccion = DateTime.Now.AddDays(1);
    [ObservableProperty] private DateTime _fechaMatriculacion = DateTime.Now.AddYears(-1);
    
    // Propiedades para Buscar
    [ObservableProperty] private string _inputMatricula = string.Empty;
    [ObservableProperty] private string _inputDni = string.Empty;
    [ObservableProperty] private string _inputMarca = string.Empty;
    [ObservableProperty] private string _inputModelo = string.Empty;
    [ObservableProperty] private TipoMotor? _inputMotor = null;
    [ObservableProperty] private DateTime? _inputFechaPrincipio = null;
    [ObservableProperty] private DateTime? _inputFechaFinal = null;
    
    [ObservableProperty] private int _paginaActual = 1;
    
    /// <summary>
    /// Colección observable que se vincula a la vista
    /// </summary>
    public ObservableCollection<Cita> Citas { get; } = new();
    /// <summary>
    ///  Lista de los distintos tipos de motor para que utilicen las comboboxes
    /// </summary>
    public List<TipoMotor> ListaMotores { get; }
    
    
    /// <summary>
    /// Constructor para inyectar kas dependencias necesarias
    /// </summary>
    public MainViewModel(CitaService citaService, ILogger<MainViewModel> logger, IConfiguration config, IInformeService informeService)
    {
        ListaMotores = new List<TipoMotor>
        {
            TipoMotor.Gasolina,
            TipoMotor.Diesel,
            TipoMotor.Electrico,
            TipoMotor.Hibrido
        };
        
        _citaService = citaService;
        _logger = logger;
        _informeService = informeService;
        // Configuración de borrado logico del appsettings.json
        _borradoLogico = bool.Parse(config["Database:LogicalDelete"] ?? "true");
        
        CargarCitas();
    }
    
    /// <summary>
    /// Método privado que coge los datos del servicio y actualiza la colección observable
    /// </summary>
    private void CargarCitas()
    {
        if (_citaService == null) return;

        try
        {
            // Convierte los filtros vacios a null, si todos los filtros son null trae todos los registros
            var lista = _citaService.Consultar(
                matricula: string.IsNullOrWhiteSpace(InputMatricula) ? null : InputMatricula,
                dni: string.IsNullOrWhiteSpace(InputDni) ? null : InputDni,
                marca: string.IsNullOrWhiteSpace(InputMarca) ? null : InputMarca,
                modelo: string.IsNullOrWhiteSpace(InputModelo) ? null : InputModelo,
                tipoMotor: InputMotor,
                fechaPrincipio: InputFechaPrincipio,
                fechaFinal: InputFechaFinal,
                pagina: PaginaActual
            );
            
            // Vacia la coleccion observable y le introduce los registros para mostrarlos en la interfaz
            Citas.Clear();
            foreach (var cita in lista)
            {
                Citas.Add(cita);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al cargar citas");
            MessageBox.Show($"Error al cargar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    /// <summary>
    /// Comando para guardar una nueva cita. Valida la creación y limpia los campos si tiene éxito.
    /// </summary>
    [RelayCommand]
    private void Create()
    {
        var cita = new Cita
        {
            Dni = Dni, Matricula = Matricula, Marca = Marca, Modelo = Modelo,
            Motor = Motor, FechaInspeccion = FechaInspeccion, FechaMatriculacion = FechaMatriculacion
        };
        try
        {
            _citaService.Create(cita);
            Log.Information("Cita guardada con éxito");
            MessageBox.Show("Cita guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Vacia las cajas de para introducir texto de la interfaz
            Dni = string.Empty;
            Matricula = string.Empty;
            Marca = string.Empty;
            Modelo = string.Empty;
            
            CargarCitas();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al guardar cita con id {Id}", cita.Id);
            MessageBox.Show(ex.Message, "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    /// <summary>
    /// Vuelve a la primera página y muestra una consulta con los filtros actuales
    /// </summary>
    [RelayCommand]
    private void Consultar()
    {
        PaginaActual = 1;
        CargarCitas();
    }
    
    /// <summary>
    /// Abre una ventana de edición para la cita elegida
    /// </summary>
    [RelayCommand]
    private void Update(Cita? cita)
    {
        if (cita == null) return;
        
        // Crea una copia para editar para no modificar la original directamente
        var citaCopia = new Cita
        {
            Id = cita.Id,
            Dni = cita.Dni,
            Matricula = cita.Matricula,
            Marca = cita.Marca,
            Modelo = cita.Modelo,
            Motor = cita.Motor,
            FechaInspeccion = cita.FechaInspeccion,
            FechaMatriculacion = cita.FechaMatriculacion
        };
        
        // Crea la vista de update y le introduce los datos de la copia
        var vista = new UpdateCitaView();
        vista.DataContext = citaCopia; 
        
        // Abre la vista, si se pulsa guardar recibe true y actualiza y cierra la vista, si se cierra la vista recibe false
        if (vista.ShowDialog() == true)
        {
            try
            {
                _citaService.Update(citaCopia, citaCopia.Id);
            
                Log.Information("Cita Actualizada correctamente");
                MessageBox.Show("Cita actualizada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarCitas();
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error al actualizar cita con id {Id}", cita.Id);
                MessageBox.Show(ex.Message, "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    /// <summary>
    /// Elimina la cita seleccionada utilizando borrado lógico o físico
    /// </summary>
    [RelayCommand]
    private void Delete(Cita? cita)
    {
        if (cita == null) return;

        try
        {
            _citaService.Delete(cita.Id, _borradoLogico);
            Log.Information("Cita eliminada correctamente");
            MessageBox.Show("Cita eliminada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            CargarCitas();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al eliminar cita con id {Id}", cita.Id);
            MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Exporta la información de la cita a un documento externo
    /// </summary>
    /// <param name="cita">La Cita a exportar</param>
    [RelayCommand]
    private void Exportar(Cita? cita)
    {
        if (cita == null) return;
        
        // Ruta del directorio en la carpeta de la aplicación
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exportaciones");
        
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        string file = $"Cita_{cita.Id}_{DateTime.Now:dd-MM-yyyy}.pdf";
        string directorio = Path.Combine(path, file);

        try
        {
            _informeService.Generar(cita, directorio);
            Log.Information("Documento generado en {Path}", directorio);
            MessageBox.Show($"Cita exportada correctamente en:\n{directorio}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al generar el documento");
            MessageBox.Show("Error al generar el documento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Pasa a la siguiente página
    /// </summary>
    [RelayCommand]
    private void PaginaSiguiente()
    {
        PaginaActual++;
        CargarCitas();
    }

    /// <summary>
    /// Vuelve a la anterior página
    /// </summary>
    [RelayCommand]
    private void PaginaAnterior()
    {
        if (PaginaActual > 1)
        {
            PaginaActual--;
            CargarCitas();
        }
    }
    
    /// <summary>
    /// Cierra la app
    /// </summary>
    [RelayCommand]
    private void Salir()
    {
        Application.Current.Shutdown();
    }

    /// <summary>
    /// Muestra la vista acerca de
    /// </summary>
    [RelayCommand]
    private void AcercaDe()
    {
        var view = new AcercaDeView();
        view.ShowDialog();
    }
}