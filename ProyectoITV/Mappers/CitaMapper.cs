using ProyectoITV.DTO;
using ProyectoITV.Enums;
using ProyectoITV.Models;

namespace ProyectoITV.Mappers;

/// <summary>
/// Clase encargada de mapear las citas a DTOs o a la inversa
/// </summary>
public class CitaMapper : IMapper<Cita, CitaDTO>
{
    /// <summary>
    /// Convierte una Cita en un CitaDTO
    /// </summary>
    /// <param name="cita">Modelo de cita</param>
    /// <returns>Objeto CitaDTO</returns>
    public CitaDTO ToDto(Cita cita)
    {
        return new CitaDTO(cita.Id, cita.Dni, cita.Matricula, cita.FechaInspeccion.ToShortDateString(), cita.Marca,
            cita.Modelo, cita.Motor.ToString(), cita.FechaMatriculacion.ToShortDateString());
    }
    
    /// <summary>
    /// Convierte un CitaDTO en una Cita
    /// </summary>
    /// <param name="dto">Dto de cita</param>
    /// <returns>Objeto de modelo    de cita</returns>
    public Cita ToModel(CitaDTO dto)
    {
        return new Cita
        {
            Id = dto.Id, Dni = dto.Dni, Matricula = dto.Matricula,
            FechaInspeccion = DateTime.Parse(dto.FechaInspeccion), Marca = dto.Marca,
            Modelo = dto.Modelo, Motor = (TipoMotor)Enum.Parse(typeof(TipoMotor), dto.Motor),
            FechaMatriculacion = DateTime.Parse(dto.FechaMatriculacion)
        };
    }
}