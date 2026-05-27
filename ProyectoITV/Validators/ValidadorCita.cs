using System.Text.RegularExpressions;

namespace ProyectoITV.Validators;

/// <summary>
/// Proporciona métodos estáticos para validar las citas
/// </summary>
public static class ValidadorCita
{
    /// <summary>
    /// Valida el formato y la letra de control de un DNI español
    /// </summary>
    /// <param name="dni">DNI</param>
    /// <returns>True si el formato y la letra son correctas, si no false</returns>
    public static bool ValidarDni(string? dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return false;
        
        dni = dni.Trim().ToUpper();
        // Regex para 8 números y 1 letra
        var regex = new Regex(@"^[0-9]{8}[A-Z]{1}$");
        
        if(!regex.IsMatch(dni)) return false;
        
        // Cálculo de la letra final según el algoritmo del Ministerio de Interior
        var numeros = int.Parse(dni.Substring(0, 8));
        var letra = dni[8];

        string letrasOrden = "TRWAGMYFPDXBNJZSQVHLCKE"; //0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 (RESTO)
        var letraCorrecta = letrasOrden[numeros % 23];
        
        return letra == letraCorrecta;
    }

    /// <summary>
    /// Valida el formato de una matrícula española
    /// </summary>
    /// <param name="matricula">Matrícula</param>
    /// <returns>True si el formato es correcto, si no false</returns>
    public static bool ValidarMatricula(string? matricula)
    {
        if (string.IsNullOrWhiteSpace(matricula)) return false;
        
        matricula = matricula.Trim().ToUpper();
        /// Regex para 4 números 3 letras
        var regex = new Regex(@"^[0-9]{4}[A-Z]{3}$");
        
        return regex.IsMatch(matricula);
    }
    
    /// <summary>
    /// Valida que la fecha de inspección esté en un rango desde hoy hasta dentro de 30 días
    /// </summary>
    /// <param name="fecha">Fecha a validar</param>
    public static bool ValidarFechaInspeccion(DateTime? fecha)
    {
        if(!fecha.HasValue) return false;

        return (fecha < DateTime.Now.AddDays(+30) && fecha >= DateTime.Now);
    }
    
    /// <summary>
    /// Valida que la fecha de matriculación no sea en el futuro
    /// </summary>
    /// <param name="fecha">Fecha de matriculación</param>
    public static bool ValidarFechaMatriculacion(DateTime? fecha)
    {
        if (!fecha.HasValue) return false;
        
        return (fecha <= DateTime.Now);
    }
}