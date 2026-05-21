using PoryectoFinal.DTO;
using PoryectoFinal.Interfaces;
using PoryectoFinal.Models;

namespace PoryectoFinal.Storage;

public class CSVStorage(IVehiculoRepository vehiculoRepo) : IStorage<CitaDTO>
{
    public void Exportar(List<CitaDTO> datos, string path)
    {
        try
        {
            using var writer = new StreamWriter(path);
            writer.Write("id,dni,matricula,marca,modelo,motor,fechaInspeccion,fechaMatriculacion");
            
            foreach (var dato in datos)
            {
                var vehiculo = vehiculoRepo.GetByMatricula(dato.Matricula);
                
                string marca = vehiculo?.Marca ?? "-";
                string modelo = vehiculo?.Modelo ?? "-";
                string motor = vehiculo?.Motor.ToString() ?? "-";
                string fechaMatriculacion = vehiculo?.FechaMatriculacion.ToShortDateString() ?? "-";
                
                writer.WriteLine($"{dato.Id},{dato.Dni},{dato.Matricula},{marca},{modelo},{motor},{dato.FechaInspeccion},{fechaMatriculacion}");
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
                var x = new CitaDTO(int.Parse(partes[0]), partes[1], partes[2], partes[6]);
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