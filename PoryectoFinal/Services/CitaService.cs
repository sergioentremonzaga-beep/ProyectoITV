using Microsoft.Extensions.Options;
using PoryectoFinal.Interfaces;
using PoryectoFinal.Models;

namespace PoryectoFinal.Services;

public class CitaService(ICitaRepository citaRepo) : ICitaService
{
    public Cita? GetById(int id)
    {
        if (id <= 0) throw new ArgumentException("El id tiene que ser mayor que 0");
        return citaRepo.GetById(id);
    }

    public void Create(Cita cita)
    {
        ValidarCita(cita);
        
        var citasVehiculoMismoDia = citaRepo.Consultar(cita.Matricula, null, null, null ,null , cita.FechaInspeccion, null, 1);

        if (citasVehiculoMismoDia.Count != 0) throw new ArgumentException("No pueden registrarse dos citas para el mismo vehiculo el mismo dia");
        
        var citasPropietarioMismoDia = citaRepo.Consultar(null, cita.Dni, null, null ,null , cita.FechaInspeccion, null, 1);
        
        if(citasPropietarioMismoDia.Count >= 3) throw new ArgumentException("No pueden registrarse mas de 3 citas para el mismo propietario el mismo dia");
        
        citaRepo.Create(cita);
    }

    public void Update(Cita cita, int id)
    {
        if (id <= 0) throw new ArgumentException("El id tiene que ser mayor que 0");
        
        var citaExists = citaRepo.GetById(id);
        if (citaExists == null) throw new ArgumentException("No existe cita para esta id");
        
        cita.Dni ??= citaExists.Dni;
        cita.Matricula ??= citaExists.Matricula;
        cita.Marca ??= citaExists.Marca;
        cita.Modelo ??= citaExists.Modelo;
        
        ValidarCita(cita);
        
        citaRepo.Update(cita, id);
    }

    public void Delete(int id, bool borradoLogico)
    {
        if (id <= 0) throw new ArgumentException("El id tiene que ser mayor que 0");
        
        var citaExists = citaRepo.GetById(id);
        if (citaExists == null) throw new ArgumentException("No existe cita para esta id");
        
        citaRepo.Delete(id, borradoLogico);
    }

    public List<Cita> Consultar(string? matricula, string? dni, string? marca, string? modelo, TipoMotor? tipoMotor, DateTime? fechaPrincipio, DateTime? fechaFinal, int pagina)
    {
        if(pagina <= 0) pagina = 1;

        return citaRepo.Consultar(matricula, dni, marca, modelo, tipoMotor, fechaPrincipio, fechaFinal, pagina);
    }
    
    private static void ValidarCita(Cita cita)
    {
        if (cita == null) throw new ArgumentException("La cita no puede ser nula.");
        
        if(cita.IsDeleted) throw new ArgumentException("La cita no puede estar eliminada");
        
        if (!ValidadorCita.ValidarDni(cita.Dni)) throw new ArgumentException("El DNI del propietario es obligatorio y tiene que ser valido");

        if (!ValidadorCita.ValidarMatricula(cita.Matricula)) throw new ArgumentException("La matricula del vehiculo es obligatoria y tiene que ser valida");

        if (!ValidadorCita.ValidarFechaInspeccion(cita.FechaInspeccion)) throw new ArgumentException("La fecha de inspeccion es obligatoria y tiene que estar comprendida entre hoy y dentro de 30 dias");
        
        if (!ValidadorCita.ValidarFechaMatriculacion(cita.FechaMatriculacion)) throw new ArgumentException("La fecha de matriculacion es obligatoria y tiene que estar comprendida entre el presente y el pasado");
    }
}