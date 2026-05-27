using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace ProyectoITV.Front;

/// <summary>
/// Vista que muestra información de la aplicación y del autor
/// </summary>
public partial class AcercaDeView : Window
{
    public AcercaDeView()
    {
        InitializeComponent();
    }
    
    /// <summary>
    /// Maneja la navegación de links, abre el navegador predeterminado
    /// </summary>
    /// <param name="sender">El objeto que provocó el evento</param>
    /// <param name="e">Argumentos de navegación que contienen la URI</param>
    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        // ProcessStartInfo abre el navegador predeterminado
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
        {
            UseShellExecute = true
        });

        e.Handled = true;
    }
}