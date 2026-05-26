using Microsoft.Data.Sqlite;
using PoryectoFinal.Interfaces;
using PoryectoFinal.Models;

namespace PoryectoFinal.Repositories;

public class AdoCitaRepository(SqliteConnection conexion) : ICitaRepository
{
    const int tamañoPagina = 10;
    
    public Cita? GetById(int id)
    {
        using var command = conexion.CreateCommand();
        command.CommandText =  "SELECT * FROM Citas WHERE Id = @Id";
        command.Parameters.AddWithValue("Id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapCita(reader) :  null;
    }

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
        command.Parameters.AddWithValue("@FechaMatriculacion", cita.FechaMatriculacion.ToString("yyyy-MM-dd"));
        
        command.ExecuteNonQuery();
    }

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
        
        command.ExecuteNonQuery();
    }

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
            command.CommandText = "DELETE FROM Citas WHERE Id = @id;";
        }
        
        command.Parameters.AddWithValue("@Id", id);
        command.ExecuteNonQuery();
    }

    public List<Cita> Consultar(string? matricula, string? dni, string? marca, string? modelo, TipoMotor? tipoMotor,
        DateTime? fechaPrincipio, DateTime? fechaFinal, int pagina)
    {
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
        command.Parameters.AddWithValue("@Paginas", (pagina - 1) * tamañoPagina);
        
        command.CommandText = sql;
        var output = new  List<Cita>();
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            output.Add(MapCita(reader));
        }
        
        return output;
    }

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