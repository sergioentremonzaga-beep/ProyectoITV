using PoryectoFinal.Models;

namespace PoryectoFinal.Interfaces;

public interface IVehiculoService
{
    public Vehiculo? GetByMatricula(string matricula);
    public void Create(Vehiculo vehiculo);
    public void Update(Vehiculo vehiculo, string matricula);
}