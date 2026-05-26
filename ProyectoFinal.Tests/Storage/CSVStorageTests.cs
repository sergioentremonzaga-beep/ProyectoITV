using FluentAssertions;
using PoryectoFinal.DTO;
using PoryectoFinal.Storage;

namespace ProyectoFinal.Tests.Repositories;

[TestFixture]
public class CSVStorageTests
{
    private CSVStorage _storage;
    private string _rutaTemp;

    [SetUp]
    public void Setup()
    {
        _storage = new CSVStorage();
        _rutaTemp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
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
            new CitaDTO(1, "12345678Z", "1234ABC", "30/05/2026", "Toyota", "Trueno", "Gasolina", "01/01/2020"),
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
        string ruta = Path.Combine(Path.GetTempPath(), "x.csv");
        
        var resultado = _storage.Importar(ruta);
        
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}