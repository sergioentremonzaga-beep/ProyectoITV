using System.ComponentModel.DataAnnotations;

namespace PoryectoFinal.Models;

public class Vehiculo
{
    [Key]
    public string Matricula { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public TipoMotor Motor { get; set; }
    public DateTime FechaMatriculacion { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}