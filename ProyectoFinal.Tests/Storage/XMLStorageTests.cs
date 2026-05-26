using FluentAssertions;
using PoryectoFinal.DTO;
using PoryectoFinal.Storage;

namespace ProyectoFinal.Tests.Repositories;

[TestFixture]
public class XMLStorageTests
{
    private XMLStorage<CitaDTO> _storage;
    private string _rutaTemp;

    [SetUp]
    public void Setup()
    {
        _storage = new XMLStorage<CitaDTO>();
        _rutaTemp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xml");
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_rutaTemp)) File.Delete(_rutaTemp);
    }

    [Test]
    public void ExportarImportarValido()
    {
        var lista = new List<CitaDTO>
        {
            new CitaDTO(1, "12345678Z", "1234ABC", "2026-05-30", "Toyota", "Trueno", "Gasolina", "2020-01-01"),
        };
        
        _storage.Exportar(lista, _rutaTemp);
        
        var citas = _storage.Importar(_rutaTemp);
        
        citas.Should().NotBeNull();
        citas.Should().HaveCount(lista.Count);
        
        citas.Should().BeEquivalentTo(lista);
    }

    [Test]
    public void ImportarDevuelveListaVacia()
    {
        string ruta = Path.Combine(Path.GetTempPath(), "x.xml");
        
        var resultado = _storage.Importar(ruta);
        
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}