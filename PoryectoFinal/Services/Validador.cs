using System.Text.RegularExpressions;

namespace PoryectoFinal.Services;

public static class Validador
{
    public static bool ValidarDni(string? dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return false;
        
        dni = dni.Trim().ToUpper();
        var regex = new Regex(@"^[0-9]{8}[A-Z]{1}$");
        
        if(!regex.IsMatch(dni)) return false;

        var numeros = int.Parse(dni.Substring(0, 8));
        var letra = dni[8];

        string letrasOrden = "TRWAGMYFPDXBNJZSQVHLCKE"; //0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 (RESTO)
        var letraCorrecta = letrasOrden[numeros % 23];
        
        return letra == letraCorrecta;
    }

    public static bool ValidarMatricula(string? matricula)
    {
        if (string.IsNullOrWhiteSpace(matricula)) return false;
        
        matricula = matricula.Trim().ToUpper();
        var regex = new Regex(@"^[0-9]{4}[A-Z]{3}$");
        
        return regex.IsMatch(matricula);
    }

    public static bool ValidarFechaInspeccion(DateTime? fecha)
    {
        if(!fecha.HasValue) return false;

        return (fecha < DateTime.Now.AddDays(+30) && fecha >= DateTime.Now);
    }

    public static bool ValidarFechaMatriculacion(DateTime? fecha)
    {
        if (!fecha.HasValue) return false;
        
        return (fecha < DateTime.Now.AddDays(+1));
    }
}