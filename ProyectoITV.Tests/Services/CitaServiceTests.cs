using FluentAssertions;
using Moq;
using ProyectoITV.Enums;
using ProyectoITV.Models;
using ProyectoITV.Repositories;
using ProyectoITV.Services;

namespace ProyectoITV.Tests.Services;

[TestFixture]
public class CitaServiceTests
{
    private Mock<ICitaRepository> _mockRepo;
    private CitaService _servicio;
    
    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<ICitaRepository>();
        _servicio = new CitaService(_mockRepo.Object);
    }

    [Test]
    public void CrearCitaValida()
    {
        var cita = new Cita
        {
            Dni = "12345678Z", Matricula = "1234ABC", Marca = "Toyota", Modelo = "Trueno",
            FechaMatriculacion = DateTime.Now.AddYears(-2), FechaInspeccion = DateTime.Now.AddDays(+1),
            Motor = TipoMotor.Gasolina
        };
        
        _mockRepo.Setup(r => r.Consultar(It.IsAny<string>(), null, null, null, null, It.IsAny<DateTime>(), null, 1))
            .Returns(new List<Cita>());
        
        _mockRepo.Setup(r => r.Consultar(null, It.IsAny<string>(), null, null, null, It.IsAny<DateTime>(), null, 1))
            .Returns(new List<Cita>());
        
        _servicio.Create(cita);
        
        _mockRepo.Verify(r => r.Create(cita), Times.Once);
    }
    
    [Test]
    [TestCase("12345678A", "1234ABC", "Toyota", "Trueno", 2, TipoMotor.Gasolina, -365, false, "El DNI del propietario es obligatorio y tiene que ser valido")]
    [TestCase("12345678Z", "123AABC", "Toyota", "Trueno", 2, TipoMotor.Gasolina, -365, false, "La matricula del vehiculo es obligatoria y tiene que ser valida")]
    [TestCase("12345678Z", "1234ABC", "Toyota", "Trueno", 40, TipoMotor.Gasolina, -365, false, "La fecha de inspeccion es obligatoria y tiene que estar comprendida entre hoy y dentro de 30 dias")]
    [TestCase("12345678Z", "1234ABC", "Toyota", "Trueno", 2, TipoMotor.Gasolina, 5, false, "La fecha de matriculacion es obligatoria y tiene que estar comprendida entre el presente y el pasado")]
    [TestCase("12345678Z", "1234ABC", "Toyota", "Trueno", 2, TipoMotor.Gasolina, -365, true, "La cita no puede estar eliminada")]
    public void CrearCitaInvlida(string? dni, string? matricula, string? marca, string? modelo, int diasInspeccion, TipoMotor motor, int diasMatriculacion, bool deleted, string mensaje)
    {
        var cita = new Cita
        {
            Dni = dni, Matricula = matricula, Marca = marca, Modelo = modelo,
            FechaMatriculacion = DateTime.Now.AddDays(diasMatriculacion), FechaInspeccion = DateTime.Now.AddDays(diasInspeccion),
            Motor = motor, IsDeleted = deleted
        };

        var excepcion = Assert.Throws<ArgumentException>(() => _servicio.Create(cita));
        excepcion.Message.Should().Contain(mensaje);
    }

    [Test]
    public void CrearCitaInvalidaNull()
    {
        Cita? cita = null;
        
        var excepcion = Assert.Throws<ArgumentException>(() => _servicio.Create(cita));
        
        excepcion.Message.Should().Contain("La cita no puede ser nula.");
    }

    [Test]
    public void CrearCitaInvalidaLimiteVehiculos()
    {
        var matricula = "1234ABC";
        var fechaInspeccion = DateTime.Now.AddDays(1);
        
        var cita = new Cita 
        { 
            Matricula = matricula,
            FechaInspeccion = fechaInspeccion,
            FechaMatriculacion = DateTime.Now.AddDays(-1),
            Dni = "12345678Z"
        };
        
        var lista = new  List<Cita>() {new Cita()};
        
        _mockRepo.Setup(r => r.Consultar(matricula, null, null, null, null, fechaInspeccion, null, 1)).Returns(lista);
        
        var excepcion = Assert.Throws<ArgumentException>(() => _servicio.Create(cita));

        excepcion.Message.Should().Contain("No pueden registrarse dos citas para el mismo vehiculo el mismo dia");
    }
    
    [Test]
    public void CrearCitaInvalidaLimitePorPersona()
    {
        var dni = "12345678Z";
        var matricula = "1234ABC";
        var fechaInspeccion = DateTime.Now.AddDays(1);
        
        var cita = new Cita 
        { 
            Matricula = matricula,
            FechaInspeccion = fechaInspeccion,
            FechaMatriculacion = DateTime.Now.AddDays(-1),
            Dni = dni
        };
        
        _mockRepo.Setup(r => r.Consultar(matricula, null, null, null, null, fechaInspeccion, null, 1))
            .Returns(new List<Cita>());
        
        var lista = new  List<Cita>() {new Cita(), new Cita(), new Cita()};
        
        _mockRepo.Setup(r => r.Consultar(null, dni, null, null, null, fechaInspeccion, null, 1)).Returns(lista);
        
        var excepcion = Assert.Throws<ArgumentException>(() => _servicio.Create(cita));

        excepcion.Message.Should().Contain("No pueden registrarse mas de 3 citas para el mismo propietario el mismo dia");
    }

    [Test]
    public void UpdateCitaValido()
    {
        var cita = new Cita
        {
            Dni = "12345678Z", Matricula = "1234ABC", Marca = "Toyota", Modelo = "Trueno",
            FechaMatriculacion = DateTime.Now.AddYears(-2), FechaInspeccion = DateTime.Now.AddDays(+1),
            Motor = TipoMotor.Gasolina
        };
        _mockRepo.Setup(r => r.GetById(1)).Returns(cita);

        var citaMod = new Cita
        {
            Id = 1,
            Matricula = "1234ZZZ",
        };
        
        _servicio.Update(citaMod, 1);
        _mockRepo.Verify(r => r.Update(citaMod, 1), Times.Once);
    }
    
    [Test]
    public void UpdateCitaInvalidoNull()
    {
        _mockRepo.Setup(r=> r.GetById(5)).Returns((Cita?)null);
        
        var citaMod = new Cita
        {
            Id = 5,
            Matricula = "1234ZZZ",
        };
        
        var excepcion = Assert.Throws<ArgumentException>(() => _servicio.Update(citaMod, 5));
        excepcion.Message.Should().Contain("No existe cita para esta id");
    }

    [Test]
    public void UpdateCitaInvalidaDatos()
    {
        var cita = new Cita
        {
            Dni = "12345678Z", Matricula = "1234ABC", Marca = "Toyota", Modelo = "Trueno",
            FechaMatriculacion = DateTime.Now.AddYears(-2), FechaInspeccion = DateTime.Now.AddDays(+1),
            Motor = TipoMotor.Gasolina
        };
        _mockRepo.Setup(r => r.GetById(1)).Returns(cita);

        var citaMod = new Cita
        {
            Id = 1,
            Matricula = "123AZZZ",
        };
        
        var excepcion = Assert.Throws<ArgumentException>(() => _servicio.Update(citaMod, 1));
        excepcion.Message.Should().Contain("La matricula del vehiculo es obligatoria y tiene que ser valida");
    }
    
    [Test]
    public void UpdateCitaInvalidaId()
    {
        var citaMod = new Cita
        {
            Id = 1,
            Matricula = "123AZZZ",
        };
        
        var excepcion = Assert.Throws<ArgumentException>(() => _servicio.Update(citaMod, -1));
        excepcion.Message.Should().Contain("El id tiene que ser mayor que 0");
    }
    
    [Test]
    public void DeleteCitaInvalidoNull()
    {
        _mockRepo.Setup(r=> r.GetById(5)).Returns((Cita?)null);
        
        var excepcion = Assert.Throws<ArgumentException>(() => _servicio.Delete(5, true));
        excepcion.Message.Should().Contain("No existe cita para esta id");
    }
    
    [Test]
    public void DeleteCitaInvalidaId()
    {
        
        var excepcion = Assert.Throws<ArgumentException>(() => _servicio.Delete(-1, true));
        excepcion.Message.Should().Contain("El id tiene que ser mayor que 0");
    }
    
    [Test]
    public void DeleteCitaValido()
    {
        var cita = new Cita
        {
            Dni = "12345678Z", Matricula = "1234ABC", Marca = "Toyota", Modelo = "Trueno",
            FechaMatriculacion = DateTime.Now.AddYears(-2), FechaInspeccion = DateTime.Now.AddDays(+1),
            Motor = TipoMotor.Gasolina
        };
        _mockRepo.Setup(r => r.GetById(1)).Returns(cita);
        
        _servicio.Delete( 1, true);
        _mockRepo.Verify(r => r.Delete( 1, true), Times.Once);
    }

    [Test]
    public void ConsultarCitaValido()
    {
        var matricula = "1234ABC";
        var dni = "12345678Z";
        var lista = new List<Cita> { new Cita { Matricula = matricula, Dni = dni } };
        
        _mockRepo.Setup(r => r.Consultar(matricula, dni, null, null, null, null, null, 1))
            .Returns(lista);
        
        var resultado = _servicio.Consultar(matricula, dni, null, null, null, null, null, 1);
        
        resultado.Should().HaveCount(1);
        resultado.First().Matricula.Should().Be(matricula);
        
        _mockRepo.Verify(r => r.Consultar(matricula, dni, null, null, null, null, null, 1), Times.Once);
    }

    [Test]
    public void ConsultarCitaCorrecionPagina()
    {
        var lista = new List<Cita> { new Cita() };
    
        
        _mockRepo.Setup(r => r.Consultar(null, null, null, null, null, null, null, 1))
            .Returns(lista);
        
        var resultado = _servicio.Consultar(null, null, null, null, null, null, null, -5);
        
        _mockRepo.Verify(r => r.Consultar(null, null, null, null, null, null, null, 1), Times.Once);
    }
}