using System.Text;
using ProyectoITV.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ProyectoITV.Report;

/// <summary>
/// Genera informes de Citas
/// Soporta PDF y HTML
/// </summary>
public class InformeService : IInformeService
{
    /// <summary>
    /// Configura la licencia de QuestPDF
    /// </summary>
    public InformeService()
    { 
        QuestPDF.Settings.License = LicenseType.Community;
    } 

    /// <summary>
    /// Genera un informe a partir de los datos de la cita y lo guarda en el archivo indicado
    /// Determina el formato basándose en la extensión del archivo
    /// </summary>
    /// <param name="cita">La Cita con los datos</param>
    /// <param name="path">Ruta del archivo</param>
    public void Generar(Cita cita, string path)
    {
        string extension = Path.GetExtension(path).ToLower();
        
        /// Genera un archivo HTML con los datos de la cita
        if (extension == ".html")
        {
            var html = new StringBuilder();
            html.AppendLine("<html><body>");
            html.AppendLine($"<h1>Ficha de Cita {cita.Id}</h1>");
            html.AppendLine($"<p>DNI: {cita.Dni}</p>");
            html.AppendLine($"<p>Matrícula: {cita.Matricula}</p>");
            html.AppendLine($"<p>Marca: {cita.Marca}</p>");
            html.AppendLine($"<p>Modelo: {cita.Modelo}</p>");
            html.AppendLine($"<p><b>Motor:</b> {cita.Motor}</p>");
            html.AppendLine($"<p><b>Fecha Inspección:</b> {cita.FechaInspeccion:dd/MM/yyyy}</p>");
            html.AppendLine($"<p><b>Fecha Matriculación:</b> {cita.FechaMatriculacion:dd/MM/yyyy}</p>");
            html.AppendLine("</body></html>");
            File.WriteAllText(path, html.ToString());
        }
        /// Genera un docymento PDF con los datos de la cita
        else if (extension == ".pdf")
        {
            Document.Create(container => {
                container.Page(page => {
                    page.Content().Column(c => {
                        c.Item().Text($"Ficha de Cita {cita.Id}").FontSize(20).Bold();
                        c.Item().Text($"DNI: {cita.Dni}");
                        c.Item().Text($"Matrícula: {cita.Matricula}");
                        c.Item().Text($"Marca: {cita.Marca}");
                        c.Item().Text($"Modelo: {cita.Modelo}");
                        c.Item().Text($"Motor: {cita.Motor}");
                        c.Item().Text($"Fecha Inspección: {cita.FechaInspeccion:dd/MM/yyyy}");
                        c.Item().Text($"Fecha Matriculación: {cita.FechaMatriculacion:dd/MM/yyyy}");
                    });
                });
            }).GeneratePdf(path);
        }
    }
}