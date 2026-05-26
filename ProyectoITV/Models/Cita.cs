using System.ComponentModel.DataAnnotations;
using ProyectoITV.Enums;

namespace ProyectoITV.Models;

public class Cita
{
    [Key]
    public int Id { get; set; }
    public string Dni { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public DateTime FechaInspeccion { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public TipoMotor Motor { get; set; }
    public DateTime FechaMatriculacion { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}