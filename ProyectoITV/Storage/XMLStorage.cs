using System.Xml.Serialization;

namespace ProyectoITV.Storage;

/// <summary>
/// Gestiona la importación y exportación de ficheros JSON
/// </summary>
/// <typeparam name="T">El tipo de objeto a importar y exportar</typeparam>
///
/// 
public class XMLStorage<T> : IStorage<T>
{
    /// <summary>
    /// Serializa una lista de objetos a un archivo XML en la ruta especificada
    /// </summary>
    /// <param name="datos">Lista de objetos a exportar</param>
    /// <param name="path">Ruta del archivo donde se guardará el XML</param>
    public void Exportar(List<T> datos, string path)
    {
        try
        {
            var xml = new XmlSerializer(typeof (List<T>));
            using var writer = new StreamWriter(path);
            xml.Serialize(writer, datos);
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Error al exportar a xml: {ex.Message}");
        }
    }

    /// <summary>
    /// Lee un archivo XML y lo convierte en una lista de objetos
    /// </summary>
    /// <param name="path">Ruta del archivo XML a leer</param>
    /// <returns>La lista de objetos deserializados o una lista vacía si ocurre un error</returns>
    public List<T> Importar(string path)
    {
        if (!File.Exists(path)) return new List<T>();

        try
        {
            using var reader = new StreamReader(path);
            var serializer = new XmlSerializer(typeof(List<T>));
            return serializer.Deserialize(reader) as List<T> ?? new List<T>();
        }
        
        catch (Exception ex)
        {
            Console.WriteLine($"Error al importar de xml: {ex.Message}");
            return new List<T>();
        }
    }
}