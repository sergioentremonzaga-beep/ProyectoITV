using System.Xml.Serialization;

namespace ProyectoITV.Storage;

public class XMLStorage<T> : IStorage<T>
{
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