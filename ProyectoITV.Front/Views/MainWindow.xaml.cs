using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProyectoITV.Enums;

namespace ProyectoITV.Front;

/// <summary>
/// Vista principal de la app
/// </summary>
public partial class MainWindow : Window
{
    /// <param name="viewModel">
    /// El ViewModel inyectado por DI
    /// </param>
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        // Pone a funcionar todos los binding
        DataContext = viewModel;
    }
}