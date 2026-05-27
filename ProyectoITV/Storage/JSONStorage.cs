using System.Text.Json;

namespace ProyectoITV.Storage;

/// <summary>
/// Gestiona la importación y exportación de ficheros JSON
/// </summary>
/// <typeparam name="T">El tipo de objeto a importar y exportar</typeparam>
public class JSONStorage<T> : IStorage<T>
{
    /// <summary>
    /// Serializa una lista de objetos a un archivo JSON en la ruta especificada
    /// </summary>
    /// <param name="datos">Lista de objetos a exportar</param>
    /// <param name="path">Ruta del archivo donde se guardará el JSON</param>
    public void Exportar(List<T> datos, string path)
    {
        // Opciones de configuración para que el JSON sea legible
        var json = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        try
        {
            string serializado = JsonSerializer.Serialize(datos, json);
            File.WriteAllText(path, serializado);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al exportar a json: {ex.Message}");
        }
    }

    /// <summary>
    /// Lee un archivo JSON y lo convierte en una lista de objetos
    /// </summary>
    /// <param name="path">Ruta del archivo JSON a leer</param>
    /// <returns>La lista de objetos deserializados o una lista vacía si el archivo no existe o hay error</returns>
    public List<T> Importar(string path)
    {
        if (!File.Exists(path)) return new List<T>();

        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al importar de json: {ex.Message}");
            return new List<T>();
        }
    }
}