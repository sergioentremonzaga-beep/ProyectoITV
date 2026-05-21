using PoryectoFinal.DTO;
using PoryectoFinal.Models;

namespace PoryectoFinal.Mappers;

public class VehiculoMapper : IMapper<Vehiculo, VehiculoDTO>
{
    public Vehiculo ToModel(VehiculoDTO dto)
    {
        return new Vehiculo{ Matricula = dto.Matricula, Marca =  dto.Marca, Modelo = dto.Modelo, Motor = (TipoMotor)Enum.Parse(typeof(TipoMotor), dto.Motor), FechaMatriculacion = DateTime.Parse(dto.FechaMatriculacion) };
    }

    public VehiculoDTO ToDto(Vehiculo vehiculo)
    {
        return new VehiculoDTO(vehiculo.Matricula, vehiculo.Marca, vehiculo.Modelo, vehiculo.Motor.ToString(), vehiculo.FechaMatriculacion.ToShortDateString());
    }
}