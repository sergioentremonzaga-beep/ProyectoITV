using System.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using ProyectoITV.Data;
using ProyectoITV.Enums;
using ProyectoITV.Models;
using ProyectoITV.Repositories;

namespace ProyectoITV.Tests.Repositories;

public class DapperCitaRepositoryTests
{
    private IDbConnection _conexion;
    private DapperCitaRepository _repo;
    
    [SetUp]
    public void Setup()
    {
        _conexion = new SqliteConnection("Data Source=:memory:");
        _conexion.Open();

        DatabaseInitializer.InicializarBd(_conexion);
        
        _repo = new DapperCitaRepository(_conexion);
    }
    
    [TearDown]
    public void TearDown()
    {
        _conexion.Close();
        _conexion.Dispose();
    }
    
    [Test]
    public void CreateValido()
    {
        var cita = CrearCita("1234ABC");

        _repo.Create(cita);
        var resultado = _repo.GetById(cita.Id);

        resultado.Should().NotBeNull();
    }

    [Test]
    public void GetByIdValido()
    {
        var cita = CrearCita("1234ABC");

        _repo.Create(cita);
        var resultado = _repo.GetById(cita.Id);
        
        resultado.Matricula.Should().Be("1234ABC");
    }

    [Test]
    public void UpdateValido()
    {
        var cita = CrearCita("1234ABC");
        _repo.Create(cita);
        
        cita.Marca = "Ford";
        cita.Modelo = "Fiesta";
        _repo.Update(cita, cita.Id);

        var resultado = _repo.GetById(cita.Id);
        resultado.Marca.Should().Be("Ford");
        resultado.Modelo.Should().Be("Fiesta");
        resultado.UpdatedAt.Should().NotBeNull();
    }
    
    [Test]
    public void DeleteLogicoNoElimina()
    {
        var cita = CrearCita("1234ABC");
        _repo.Create(cita);

        _repo.Delete(cita.Id,true);
        
        var resultado = _repo.GetById(cita.Id);
        resultado.Should().NotBeNull();
        resultado.IsDeleted.Should().BeTrue();
    }

    [Test]
    public void DeleteFisicoElimina()
    {
        var cita = CrearCita("1234ABC");
        _repo.Create(cita);

        _repo.Delete(cita.Id, false);

        var resultado = _repo.GetById(cita.Id);
        resultado.Should().BeNull();
    }

    [TestCase("1111AAA", null, null, null, null, 1, 1)]
    [TestCase(null, "11111111H", null, null, null, 1, 2)]
    [TestCase(null, null, "Ford", null, null, 1, 1)]
    [TestCase(null, null, null, "Civic", null, 1, 1)]
    [TestCase(null, null, null, null, TipoMotor.Gasolina, 1, 2)]
    [TestCase(null, null, null, null, null, 2, 0)]
    [TestCase("9999ZZZ", null, null, null, null, 1, 0)]
    public void TestsConsultar(string? matricula, string? dni, string? marca, string? modelo, TipoMotor? motor,
        int pagina, int esperado)
    {
        _repo.Create(new Cita
        {
            Matricula = "1111AAA", Dni = "12345678Z", Marca = "Toyota", Modelo = "Trueno",
            Motor = TipoMotor.Gasolina, FechaInspeccion = DateTime.Now, FechaMatriculacion = DateTime.Now
        });
        _repo.Create(new Cita
        {
            Matricula = "2222BBB", Dni = "11111111H", Marca = "Honda", Modelo = "Civic",
            Motor = TipoMotor.Gasolina, FechaInspeccion = DateTime.Now, FechaMatriculacion = DateTime.Now
        });
        _repo.Create(new Cita
        {
            Matricula = "3333CCC", Dni = "11111111H", Marca = "Ford", Modelo = "Fiesta",
            Motor = TipoMotor.Diesel, FechaInspeccion = DateTime.Now, FechaMatriculacion = DateTime.Now
        });
        _repo.Create(new Cita
        {
            Matricula = "4444DDD", Dni = "55555555K", Marca = "Seat", Modelo = "Ibiza",
            Motor = TipoMotor.Hibrido, FechaInspeccion = DateTime.Now, IsDeleted = true,
            FechaMatriculacion = DateTime.Now
        });

        var resultado = _repo.Consultar(matricula, dni, marca, modelo, motor, null, null, pagina);

        resultado.Count().Should().Be(esperado);
    }
    
    private Cita CrearCita(string matricula)
    {
        return new Cita
        {
            Dni = "12345678Z",
            Matricula = matricula,
            FechaInspeccion = DateTime.Now.AddDays(1),
            Marca = "Toyota",
            Modelo = "Trueno",
            Motor = TipoMotor.Gasolina,
            FechaMatriculacion = DateTime.Now.AddDays(-5)
        };
    }
}