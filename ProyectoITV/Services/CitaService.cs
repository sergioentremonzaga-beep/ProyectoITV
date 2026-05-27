
using Microsoft.Extensions.Logging;
using ProyectoITV.Enums;
using ProyectoITV.Models;
using ProyectoITV.Repositories;
using ProyectoITV.Validators;
using Serilog;


namespace ProyectoITV.Services;


/// <summary>
/// Proporciona la lógica de negocio para la gestión de las citas
/// Sirve para comunicar la capa de datos con la interfaz gráfica
/// </summary>
public class CitaService(ICitaRepository citaRepo) : ICitaService
{
    /// <summary>
    /// Recupera una cita por su ID, validando que sea válida
    /// </summary>
    public Cita? GetById(int id)
    {
        if (id <= 0) throw new ArgumentException("El id tiene que ser mayor que 0");
        return citaRepo.GetById(id);
    }

    /// <summary>
    /// Crea una nueva cita tras verificar que cumpla las condiciones
    /// </summary>
    /// <exception cref="ArgumentException">Lanzada si las reglas de validación o límites fallan.</exception>
    public void Create(Cita cita)
    {
        ValidarCita(cita);
        
        // Un vehículo no puede tener dos citas el mismo día
        var citasVehiculoMismoDia = citaRepo.Consultar(cita.Matricula, null, null, null ,null , cita.FechaInspeccion, null, 1);

        if (citasVehiculoMismoDia.Count != 0) throw new ArgumentException("No pueden registrarse dos citas para el mismo vehiculo el mismo dia");
        
        // Una persona tiene un límite de 3 citas por día
        var citasPropietarioMismoDia = citaRepo.Consultar(null, cita.Dni, null, null ,null , cita.FechaInspeccion, null, 1);
        
        if(citasPropietarioMismoDia.Count >= 3) throw new ArgumentException("No pueden registrarse mas de 3 citas para el mismo propietario el mismo dia");
        
        citaRepo.Create(cita);
    }

    /// <summary>
    /// Actualiza una cita existente
    /// </summary>
    public void Update(Cita cita, int id)
    {
        if (id <= 0) throw new ArgumentException("El id tiene que ser mayor que 0");
        
        var citaExists = citaRepo.GetById(id);
        if (citaExists == null) throw new ArgumentException("No existe cita para esta id");
        
        // Si el valor recibido es nulo, mantiene el valor de la base de datos
        if (string.IsNullOrEmpty(cita.Dni)) cita.Dni = citaExists.Dni;
        if (string.IsNullOrEmpty(cita.Matricula)) cita.Matricula = citaExists.Matricula;
        if (string.IsNullOrEmpty(cita.Marca)) cita.Marca = citaExists.Marca;
        if (string.IsNullOrEmpty(cita.Modelo)) cita.Modelo = citaExists.Modelo;
        if (cita.FechaInspeccion == default) cita.FechaInspeccion = citaExists.FechaInspeccion;
        if (cita.FechaMatriculacion == default) cita.FechaMatriculacion = citaExists.FechaMatriculacion;
        
        ValidarCita(cita);
        
        citaRepo.Update(cita, id);
    }

    /// <summary>
    /// Elimina una cita de forma física o lógica si existe
    /// </summary>
    public void Delete(int id, bool borradoLogico)
    {
        if (id <= 0) throw new ArgumentException("El id tiene que ser mayor que 0");
        
        var citaExists = citaRepo.GetById(id);
        if (citaExists == null) throw new ArgumentException("No existe cita para esta id");
        
        citaRepo.Delete(id, borradoLogico);
    }

    /// <summary>
    /// Realiza una consulta filtrada y con paginación de los resultados
    /// </summary>
    public List<Cita> Consultar(string? matricula, string? dni, string? marca, string? modelo, TipoMotor? tipoMotor, DateTime? fechaPrincipio, DateTime? fechaFinal, int pagina)
    {
        if(pagina <= 0) pagina = 1;

        return citaRepo.Consultar(matricula, dni, marca, modelo, tipoMotor, fechaPrincipio, fechaFinal, pagina);
    }
    
    /// <summary>
    /// Método privado estático de validación de citas
    /// </summary>
    private static void ValidarCita(Cita cita)
    {
        // Verifica que no sea nula
        if (cita == null) throw new ArgumentException("La cita no puede ser nula.");
        
        // Verifica que no este borrada lógicamente
        if(cita.IsDeleted) throw new ArgumentException("La cita no puede estar eliminada");
        
        if (!ValidadorCita.ValidarDni(cita.Dni)) throw new ArgumentException("El DNI del propietario es obligatorio y tiene que ser valido");

        if (!ValidadorCita.ValidarMatricula(cita.Matricula)) throw new ArgumentException("La matricula del vehiculo es obligatoria y tiene que ser valida");

        if (!ValidadorCita.ValidarFechaInspeccion(cita.FechaInspeccion)) throw new ArgumentException("La fecha de inspeccion es obligatoria y tiene que estar comprendida entre hoy y dentro de 30 dias");
        
        if (!ValidadorCita.ValidarFechaMatriculacion(cita.FechaMatriculacion)) throw new ArgumentException("La fecha de matriculacion es obligatoria y tiene que estar comprendida entre el presente y el pasado");
    }
}