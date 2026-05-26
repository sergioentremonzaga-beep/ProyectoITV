using PoryectoFinal.DTO;
using PoryectoFinal.Interfaces;
using PoryectoFinal.Models;

namespace PoryectoFinal.Storage;

public class CSVStorage() : IStorage<CitaDTO>
{
    public void Exportar(List<CitaDTO> datos, string path)
    {
        try
        {
            using var writer = new StreamWriter(path);
            writer.Write("id,dni,matricula,marca,modelo,motor,fechaInspeccion,fechaMatriculacion");
            
            foreach (var dato in datos)
            {
                writer.WriteLine($"{dato.Id},{dato.Dni},{dato.Matricula},{dato.Marca},{dato.Modelo},{dato.Motor.ToString()},{dato.FechaInspeccion},{dato.FechaMatriculacion}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al exportar a csv: {ex.Message}");
        }
    }

    public List<CitaDTO> Importar(string path)
    {
        List<CitaDTO> output = new();
        try
        {
            using var reader = new StreamReader(path);
            string? line = reader.ReadLine();

            while ((line = reader.ReadLine()) != null)
            {
                string[] partes = line.Split(',');
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