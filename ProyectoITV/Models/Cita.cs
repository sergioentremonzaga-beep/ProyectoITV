using System.ComponentModel.DataAnnotations;
using ProyectoITV.Enums;

namespace ProyectoITV.Models;

/// <summary>
/// Representa una cita para la ITV
/// </summary>
public class Cita
{
    // Identificador único de la cita (Primary Key)
    [Key]
    public int Id { get; set; }
    /// </summary>
    /// /// <param name="Id">Identificador único de la cita</param>
    /// <param name="Dni">DNI del propietario.</param>
    /// <param name="Matricula">Matrícula del vehículo</param>
    /// <param name="FechaInspeccion">Fecha de la inspección</param>
    /// <param name="Marca">Marca del vehículo</param>
    /// <param name="Modelo">Modelo del vehículo</param>
    /// <param name="Motor">Tipo del motor.</param>
    /// <param name="FechaMatriculacion">Fecha de la matriculación del vehículo</param>
    /// <param name="IsDeleted">Si el objeto ha sido borrado lógicamente o no</param>
    /// <param name="CreatedAt">Fecha de creación del objeto></param>
    /// <param name="UpdatedAt">Fecha de la ultima actualización del objeto</param>
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