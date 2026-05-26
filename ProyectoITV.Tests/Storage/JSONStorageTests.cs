using FluentAssertions;
using ProyectoITV.DTO;
using ProyectoITV.Storage;

namespace ProyectoITV.Tests.Storage;

[TestFixture]
public class JSONStorageTests
{
    private JSONStorage<CitaDTO> _storage;
    private string _rutaTemp;

    [SetUp]
    public void Setup()
    {
        _storage = new JSONStorage<CitaDTO>();
        _rutaTemp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
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
        string ruta = Path.Combine(Path.GetTempPath(), "x.json");
        
        var resultado = _storage.Importar(ruta);
        
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}