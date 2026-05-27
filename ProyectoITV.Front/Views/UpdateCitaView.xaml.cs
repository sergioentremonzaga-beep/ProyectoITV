using System.Windows;

namespace ProyectoITV.Front;

public partial class UpdateCitaView : Window
{
    public UpdateCitaView()
    {
        InitializeComponent();
    }
    
    private void Guardar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}