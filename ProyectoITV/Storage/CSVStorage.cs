using ProyectoITV.DTO;

namespace ProyectoITV.Storage;

/// <summary>
/// Gestiona la importación y exportación de ficheros CSV
/// </summary>
public class CSVStorage() : IStorage<CitaDTO>
{
    /// <summary>
    /// Exporta una lista de objetos CitaDTO a un archivo CSV en la ruta especificada
    /// </summary>
    /// <param name="datos">Lista de citas a exportar</param>
    /// <param name="path">Ruta del archivo donde se guardará el CSV</param>
    public void Exportar(List<CitaDTO> datos, string path)
    {
        try
        {
            using var writer = new StreamWriter(path);
            // Escribe el encabezado del CSV
            writer.WriteLine("id;dni;matricula;marca;modelo;motor;fechaInspeccion;fechaMatriculacion");
            
            foreach (var dato in datos)
            {
                writer.WriteLine($"{dato.Id};{dato.Dni};{dato.Matricula};{dato.Marca};{dato.Modelo};{dato.Motor.ToString()};{dato.FechaInspeccion};{dato.FechaMatriculacion}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al exportar a csv: {ex.Message}");
        }
    }

    /// <summary>
    /// Importa datos desde un archivo CSV y los convierte en una lista de CitaDTO
    /// </summary>
    /// <param name="path">Ruta del archivo CSV a leer</param>
    /// <returns>Lista de objetos CitaDTO del archivo</returns>
    public List<CitaDTO> Importar(string path)
    {
        List<CitaDTO> output = new();
        try
        {
            using var reader = new StreamReader(path);
            // Salta la cabecera
            string? line = reader.ReadLine();

            while ((line = reader.ReadLine()) != null)
            {
                string[] partes = line.Split(';');
                var x = new CitaDTO(int.Parse(partes[0]), partes[1], partes[2], partes[6], partes[3], partes[4], partes[5], partes[7]);
                output.Add(x);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al importar de csv: {ex.Message}");
        }
        return output;
    }
}