using PoryectoFinal.DTO;
using PoryectoFinal.Interfaces;
using PoryectoFinal.Models;

namespace PoryectoFinal.Mappers;

public class CitaMapper(IVehiculoService vehiculoService) : IMapper<Cita, CitaDTO>
{
    public CitaDTO ToDto(Cita cita)
    {
        return new CitaDTO(cita.Id, cita.Dni, cita.Matricula, cita.FechaInspeccion.ToShortDateString());
    }

    public Cita ToModel(CitaDTO citaDTO)
    {
        var vehiculo = vehiculoService.GetByMatricula(citaDTO.Matricula);
        return new Cita { Id = citaDTO.Id, Dni = citaDTO.Dni, Matricula = citaDTO.Matricula, FechaInspeccion = DateTime.Parse(citaDTO.FechaInspeccion), Vehiculo = vehiculo };
    }
}