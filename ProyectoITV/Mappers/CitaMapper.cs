using ProyectoITV.DTO;
using ProyectoITV.Enums;
using ProyectoITV.Models;

namespace ProyectoITV.Mappers;

public class CitaMapper : IMapper<Cita, CitaDTO>
{
    public CitaDTO ToDto(Cita cita)
    {
        return new CitaDTO(cita.Id, cita.Dni, cita.Matricula, cita.FechaInspeccion.ToShortDateString(), cita.Marca,
            cita.Modelo, cita.Motor.ToString(), cita.FechaMatriculacion.ToShortDateString());
    }

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