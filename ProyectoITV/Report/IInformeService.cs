using ProyectoITV.Models;

namespace ProyectoITV.Report;

/// <summary>
/// Define el contrato para la generación de informes
/// </summary>
/// <remarks>
/// Esta interfaz permite implementar diferentes formatos de documento
/// </remarks>
public interface IInformeService
{
    /// <summary>
    /// Genera un informe a partir de la información de la cita y lo guarda en la ruta indicada
    /// </summary>
    /// <param name="cita">La Cita con los datos</param>
    /// <param name="path">La ruta de destino donde se guardará el informe</param>
    void Generar(Cita cita, string path);
}