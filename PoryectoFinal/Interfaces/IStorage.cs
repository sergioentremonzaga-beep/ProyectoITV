using PoryectoFinal.Models;

namespace PoryectoFinal.Interfaces;

public interface IStorage<T>
{
    public void Exportar(List<T> datos, string path);
    public List<T> Importar(string path);
}