using System.Text.Json;

namespace ProyectoITV.Storage;

public class JSONStorage<T> : IStorage<T>
{
    public void Exportar(List<T> datos, string path)
    {
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