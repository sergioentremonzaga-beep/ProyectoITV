using Microsoft.Data.Sqlite;
using ProyectoITV.Enums;
using ProyectoITV.Models;

namespace ProyectoITV.Repositories;

/// <summary>
/// Esta clase gestiona el acceso a datos utilizando ADO
/// </summary>
/// <param name="conexion">La conexión a la base de datos</param>
public class AdoCitaRepository(SqliteConnection conexion) : ICitaRepository
{
    const int tamañoPagina = 10;
    
    /// <summary>
    /// Devuelve una cita específica por su ID
    /// </summary>
    public Cita? GetById(int id)
    {
        using var command = conexion.CreateCommand();
        command.CommandText =  "SELECT * FROM Citas WHERE Id = @Id";
        command.Parameters.AddWithValue("Id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapCita(reader) :  null;
    }
    
    /// <summary>
    /// Inserta una nueva cita en la base de datos
    /// </summary>
    public void Create(Cita cita)
    {
        using var command = conexion.CreateCommand();
        command.CommandText = """
                              INSERT INTO Citas (Dni, Matricula, FechaInspeccion,  Marca, Modelo, Motor, FechaMatriculacion)
                              VALUES (@Dni, @Matricula, @FechaInspeccion, @Marca, @Modelo, @Motor, @FechaMatriculacion);
                              """;
        command.Parameters.AddWithValue("@Dni", cita.Dni);
        command.Parameters.AddWithValue("@Matricula", cita.Matricula);
        command.Parameters.AddWithValue("@FechaInspeccion", cita.FechaInspeccion.ToString("o"));
        command.Parameters.AddWithValue("@Marca", cita.Marca);
        command.Parameters.AddWithValue("@Modelo", cita.Modelo);
        command.Parameters.AddWithValue("@Motor", (int)cita.Motor);
        command.Parameters.AddWithValue("@FechaMatriculacion", cita.FechaMatriculacion.ToString("o"));
        
        command.ExecuteNonQuery();
    }
    
    /// <summary>
    /// Actualiza los datos de una cita
    /// </summary>
    public void Update(Cita cita, int id)
    {
        using var command = conexion.CreateCommand();
        command.CommandText = """
                              UPDATE Citas SET
                              Dni = @Dni,
                              Matricula = @Matricula,
                              FechaInspeccion = @FechaInspeccion,
                              Marca = @Marca,
                              Modelo = @Modelo,
                              Motor = @Motor,
                              FechaMatriculacion = @FechaMatriculacion,
                              IsDeleted = @IsDeleted,
                              UpdatedAt = @UpdatedAt
                              WHERE Id = @Id;
                              """;
        
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Dni", cita.Dni);
        command.Parameters.AddWithValue("@Matricula", cita.Matricula);
        command.Parameters.AddWithValue("@FechaInspeccion", cita.FechaInspeccion.ToString("o"));
        command.Parameters.AddWithValue("@Marca", cita.Marca);
        command.Parameters.AddWithValue("@Modelo", cita.Modelo);
        command.Parameters.AddWithValue("@Motor", (int)cita.Motor);
        command.Parameters.AddWithValue("@FechaMatriculacion", cita.FechaMatriculacion.ToString("o"));
        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("o"));
        command.Parameters.AddWithValue("@IsDeleted", cita.IsDeleted);
        
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Elimina una cita de forma física o lógica
    /// </summary>
    /// <param name="borradoLogico">Si es true, cambia IsDeleted a 1 (borrado), si es false, elimina la cita de la base de datos</param>
    public void Delete(int id, bool borradoLogico)
    {
        using var command = conexion.CreateCommand();
        if (borradoLogico)
        {
            command.CommandText = """
                                  UPDATE Citas SET
                                  IsDeleted = @IsDeleted,
                                  UpdatedAt = @UpdatedAt
                                  WHERE Id = @Id;
                                  """;
            command.Parameters.AddWithValue("@IsDeleted", 1);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("o"));
            
        }
        else
        {
            command.CommandText = "DELETE FROM Citas WHERE Id = @Id;";
        }
        
        command.Parameters.AddWithValue("@Id", id);
        command.ExecuteNonQuery();
    }
    
    /// <summary>
    /// Realiza una consulta filtrada y con paginación de los resultados
    /// </summary>
    public List<Cita> Consultar(string? matricula, string? dni, string? marca, string? modelo, TipoMotor? tipoMotor,
        DateTime? fechaPrincipio, DateTime? fechaFinal, int pagina)
    {
        // Si el parámetro no es nulo lo toma e ignora el OR, en caso de que lo sea toma el nulo
        string sql = """
                     SELECT * FROM Citas
                     WHERE IsDeleted = 0
                     AND (Matricula = @Matricula OR @Matricula IS NULL)
                     AND (Dni = @Dni OR @Dni IS NULL)
                     AND (Marca = @Marca OR @Marca IS NULL)
                     AND (Modelo = @Modelo OR @Modelo IS NULL)
                     AND (Motor = @Motor OR @Motor IS NULL)
                     AND (FechaInspeccion >= @FechaPrincipio OR @FechaPrincipio IS NULL)
                     AND (FechaInspeccion <= @FechaFinal OR @FechaFinal IS NULL)
                     ORDER BY FechaInspeccion LIMIT @Tamaño OFFSET @Paginas;
                     """;
        
        using var command = conexion.CreateCommand();
        
        command.Parameters.AddWithValue("@Matricula", string.IsNullOrWhiteSpace(matricula) ? DBNull.Value : matricula);
        command.Parameters.AddWithValue("@Dni", string.IsNullOrWhiteSpace(dni) ? DBNull.Value : dni);
        command.Parameters.AddWithValue("@Marca", string.IsNullOrWhiteSpace(marca) ? DBNull.Value : marca);
        command.Parameters.AddWithValue("@Modelo", string.IsNullOrWhiteSpace(modelo) ? DBNull.Value : modelo);
        command.Parameters.AddWithValue("@Motor", tipoMotor.HasValue ? (int)tipoMotor.Value : DBNull.Value);
        command.Parameters.AddWithValue("@FechaPrincipio", fechaPrincipio.HasValue ? fechaPrincipio.Value.ToString("o") : DBNull.Value);
        command.Parameters.AddWithValue("@FechaFinal", fechaFinal.HasValue ? fechaFinal.Value.ToString("o") : DBNull.Value);
        command.Parameters.AddWithValue("@Tamaño", tamañoPagina);
        command.Parameters.AddWithValue("@Paginas", (pagina - 1) * tamañoPagina); // Toma 10 registros por pagina y salta todos los anteriores registros (pagina-1 * 10 = todos los registros a saltar == todas las anteriores páginas)
        
        command.CommandText = sql;
        var output = new  List<Cita>();
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            output.Add(MapCita(reader));
        }
        
        return output;
    }

    /// <summary>
    /// Método auxiliar para mapear un objeto de la base de datos a una cita
    /// </summary>
    private static Cita MapCita(SqliteDataReader reader)
    {
        var motor = (TipoMotor)reader.GetInt32(6);
        
        return new Cita
        {
            Id = reader.GetInt32(0),
            Dni = reader.GetString(1),
            Matricula = reader.GetString(2),
            FechaInspeccion = DateTime.Parse(reader.GetString(3)),
            Marca = reader.GetString(4),
            Modelo = reader.GetString(5),
            Motor = motor,
            FechaMatriculacion = DateTime.Parse(reader.GetString(7)),
            IsDeleted = reader.GetInt32(8) == 1,
            CreatedAt = DateTime.Parse(reader.GetString(9)),
            UpdatedAt = reader.IsDBNull(10) ? null : DateTime.Parse(reader.GetString(10))
        };
    }
}