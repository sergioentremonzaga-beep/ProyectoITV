using PoryectoFinal.Data;
using PoryectoFinal.Interfaces;
using PoryectoFinal.Models;

namespace PoryectoFinal.Repositories;

public class EfCoreVehiculoRepository(AppDbContext context) : IVehiculoRepository
{
    public Vehiculo? GetByMatricula(string matricula)
    {
        return context.Vehiculos.Find(matricula);
    }

    public void Create(Vehiculo vehiculo)
    {
        context.Vehiculos.Add(vehiculo);
        context.SaveChanges();
    }

    public void Update(Vehiculo vehiculo, string matricula)
    {
        var exists = context.Vehiculos.Find(matricula);
        if (exists != null)
        {
            exists.UpdatedAt = DateTime.Now;
            exists.Matricula = vehiculo.Matricula;
            exists.Marca = vehiculo.Marca;
            exists.FechaMatriculacion = vehiculo.FechaMatriculacion;
            exists.Modelo = vehiculo.Modelo;
            exists.Motor = vehiculo.Motor;
            
            context.SaveChanges();
        }
    }
}