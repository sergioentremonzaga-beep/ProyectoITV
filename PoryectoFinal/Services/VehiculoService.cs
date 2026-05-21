using PoryectoFinal.Interfaces;
using PoryectoFinal.Models;

namespace PoryectoFinal.Services;

public class VehiculoService(IVehiculoRepository vehiculoRepo) : IVehiculoService
{
    public Vehiculo? GetByMatricula(string matricula)
    {
        if (!Validador.ValidarMatricula(matricula)) throw new ArgumentException("La matricula del vehiculo es obligatoria y tiene que ser valida");
        return vehiculoRepo.GetByMatricula(matricula);
    }

    public void Create(Vehiculo vehiculo)
    {
        ValidarVehiculo(vehiculo);
        
        vehiculoRepo.Create(vehiculo);
    }

    public void Update(Vehiculo vehiculo, string matricula)
    {
        if (!Validador.ValidarMatricula(matricula)) throw new ArgumentException("La matricula del vehiculo es obligatoria y tiene que ser valida");
        
        ValidarVehiculo(vehiculo);
        
        var vehiculoExists = vehiculoRepo.GetByMatricula(matricula);
        if (vehiculoExists == null)
        {
            throw new ArgumentException("No vehiculo para esta matricula");
        }
        
        vehiculoRepo.Update(vehiculo, matricula);
    }
    
    private static void ValidarVehiculo(Vehiculo vehiculo)
    {
        if (vehiculo == null) throw new ArgumentException("El vehiculo no puede ser nulo");
        
        if(vehiculo.IsDeleted) throw new ArgumentException("El vehiculo no puede estar eliminado");
        
        if (!Validador.ValidarMatricula(vehiculo.Matricula)) throw new ArgumentException("La matricula del vehiculo es obligatoria y tiene que ser valida");

        if (!Validador.ValidarFechaMatriculacion(vehiculo.FechaMatriculacion)) throw new ArgumentException("La fecha de matriculacion es obligatoria y tiene que estar comprendida entre el presente y el pasado");
    }
}