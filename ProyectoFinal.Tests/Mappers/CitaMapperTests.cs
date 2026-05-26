using FluentAssertions;
using PoryectoFinal.DTO;
using PoryectoFinal.Mappers;
using PoryectoFinal.Models;

namespace ProyectoFinal.Tests.Mappers;

[TestFixture]
public class CitaMapperTests
{
    private CitaMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _mapper = new CitaMapper();
    }
    
    [Test]
    public void ToDtoValido()
    {
        var citaModel = new Cita
        {
            Id = 1,
            Dni = "12345678Z",
            Matricula = "1234ABC",
            FechaInspeccion = DateTime.Now.AddDays(1),
            Marca = "Toyota",
            Modelo = "Trueno",
            Motor = TipoMotor.Gasolina,
            FechaMatriculacion = DateTime.Now.AddDays(-2),
            IsDeleted = false,
            CreatedAt = DateTime.Now
        };
        
        var dto = _mapper.ToDto(citaModel);
        
        dto.Should().NotBeNull();
        dto.Id.Should().Be(citaModel.Id);
        dto.Dni.Should().Be(citaModel.Dni);
        dto.Matricula.Should().Be(citaModel.Matricula);
        dto.Marca.Should().Be(citaModel.Marca);
        dto.Modelo.Should().Be(citaModel.Modelo);
        dto.Motor.Should().Be("Gasolina");
        dto.FechaInspeccion.Should().Be(citaModel.FechaInspeccion.ToShortDateString());
        dto.FechaMatriculacion.Should().Be(citaModel.FechaMatriculacion.ToShortDateString());
    }

    [Test]
    public void ToModelValido()
    {
        var citaDto = new CitaDTO(
            Id: 1,
            Dni: "12345678Z",
            Matricula: "1234ABC",
            FechaInspeccion: "2026-05-30",
            Marca: "Toyota",
            Modelo: "Trueno",
            Motor: "Gasolina",
            FechaMatriculacion: "2020-01-01"
        );
        
        var model = _mapper.ToModel(citaDto);
        
        model.Should().NotBeNull();
        model.Id.Should().Be(citaDto.Id);
        model.Dni.Should().Be(citaDto.Dni);
        model.Matricula.Should().Be(citaDto.Matricula);
        model.Marca.Should().Be(citaDto.Marca);
        model.Modelo.Should().Be(citaDto.Modelo);
        model.Motor.Should().Be(TipoMotor.Gasolina);
        model.FechaInspeccion.Should().Be(DateTime.Parse(citaDto.FechaInspeccion));
        model.FechaMatriculacion.Should().Be(DateTime.Parse(citaDto.FechaMatriculacion));
    }

    [Test]
    public void ToModelInvalido()
    {
        var citaDto = new CitaDTO(
            Id: 1, 
            Dni: "12345678Z", 
            Matricula: "1234ABC", 
            FechaInspeccion: "zzz",
            Marca: "Toyota", 
            Modelo: "Trueno", 
            Motor: "Gasolina", 
            FechaMatriculacion: "2020-01-01"
        );
        
        Action mapeo = () => _mapper.ToModel(citaDto);
        
        mapeo.Should().Throw<FormatException>();
    }
}