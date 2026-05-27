using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ProyectoITV.Data;

/// <summary>
/// Proporciona el método estáticos para la inicialización de la base de datos
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Crea la tabla Citas en la base de datos si aún no existe
    /// </summary>
    /// <param name="conexion">La interfaz de conexión a la base de datos</param>
    public static void InicializarBd(IDbConnection conexion)
    {
        string sql = """
                     CREATE TABLE IF NOT EXISTS Citas(
                         Id INTEGER PRIMARY KEY AUTOINCREMENT,
                         Dni TEXT NOT NULL,
                         Matricula TEXT NOT NULL,
                         FechaInspeccion TEXT NOT NULL,
                         Marca TEXT NOT NULL,
                         Modelo TEXT NOT NULL,
                         Motor INTEGER  NOT NULL,
                         FechaMatriculacion TEXT NOT NULL,
                         IsDeleted INTEGER NOT NULL DEFAULT 0,
                         CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                         UpdatedAt TEXT
                     );
                     """;
        conexion.Execute(sql);
    }
}