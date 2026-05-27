namespace ProyectoITV.Storage;

/// <summary>
/// Define un contrato genérico para los Storage, permite su uso independientemente del tipo de fichero
/// </summary>
/// <typeparam name="T">El tipo de objeto que se va a persistir.</typeparam>
public interface IStorage<T>
{
    /// <summary>
    /// Exporta una lista de objetos a un archivo en la ruta especificada
    /// </summary>
    /// <param name="datos">La lista de objetos a exportar</param>
    /// <param name="path">La ruta del archivo donde se guardará el fichero</param>
    public void Exportar(List<T> datos, string path);
    
    /// <summary>
    /// Importa datos desde un archivo y los convierte en una lista de objetos
    /// </summary>
    /// <param name="path">La ruta del archivo del que se leeran los datos</param>
    /// <returns>Una lista de objetos</returns>
    public List<T> Importar(string path);
}