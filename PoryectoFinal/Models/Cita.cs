namespace PoryectoFinal.Models;

public class Cita
{
    public int Id { get; set; }
    public string Dni { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public DateTime FechaInspeccion { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public Vehiculo Vehiculo { get; set; }
}