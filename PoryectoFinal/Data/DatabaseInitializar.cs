using Dapper;
using Microsoft.Data.Sqlite;

namespace PoryectoFinal.Data;

public static class DatabaseInitializar
{
    public static void InicializarBd(string conexionString)
    {
        using var conexion = new SqliteConnection(conexionString);
        conexion.Open();

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
        conexion.Close();
    }
}