using FluentAssertions;
using ProyectoITV.Validators;

namespace ProyectoITV.Tests.Validators;

[TestFixture]
public class ValidadorCitaTests
{
    [Test]
    [TestCase("12345678Z")]
    public void ValidarDniValido(string dni)
    {
        ValidadorCita.ValidarDni(dni).Should().BeTrue();
    }

    [Test]
    [TestCase("12345678A")]
    [TestCase("1234Cb78A")]
    [TestCase(null)]
    public void ValidarDniInvalido(string? dni)
    {
        ValidadorCita.ValidarDni(dni).Should().BeFalse();
    }
    
    [Test]
    [TestCase("1234ABC")]
    public void ValidarMatriculaValida(string matricula)
    {
        ValidadorCita.ValidarMatricula(matricula).Should().BeTrue();
    }

    [Test]
    [TestCase("123ABCD")]
    [TestCase(null)]
    public void ValidarMatriculaInvalida(string? matricula)
    {
        ValidadorCita.ValidarMatricula(matricula).Should().BeFalse();
    }

    [Test]
    public void ValidarFechaInspeccionValida()
    {
        var fecha = DateTime.Now.AddDays(+3);
        ValidadorCita.ValidarFechaInspeccion(fecha).Should().BeTrue();
    }
    
    [Test]
    public void ValidarFechaInspeccionInvalida()
    {
        var fecha = DateTime.Now.AddDays(+31);
        ValidadorCita.ValidarFechaInspeccion(fecha).Should().BeFalse();
    }

    [Test]
    public void ValidarFechaMatriculacionValida()
    {
        var fecha = DateTime.Now.AddDays(-1);
        ValidadorCita.ValidarFechaMatriculacion(fecha).Should().BeTrue();
    }
    
    [Test]
    public void ValidarFechaMatriculacionInvalida()
    {
        var fecha = DateTime.Now.AddDays(+1);
        ValidadorCita.ValidarFechaMatriculacion(fecha).Should().BeFalse();
    }
}