using Microsoft.Data.Sqlite;
using PoryectoFinal.Interfaces;
using PoryectoFinal.Models;

namespace PoryectoFinal.Repositories;

public class AdoCitaRepository(SqliteConnection conexion, IVehiculoRepository vehiculoRepo) : ICitaRepository
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
                              INSERT INTO Citas (Dni, Matricula, FechaInspeccion)
                              VALUES (@Dni, @Matricula, @FechaInspeccion);
                              """;
        command.Parameters.AddWithValue("@Dni", cita.Dni);
        command.Parameters.AddWithValue("@Matricula", cita.Matricula);
        command.Parameters.AddWithValue("@FechaInspeccion", cita.FechaInspeccion.ToString("o"));
        
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
                              UpdatedAt = @UpdatedAt
                              WHERE Id = @Id;
                              """;
        
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Dni", cita.Dni);
        command.Parameters.AddWithValue("@Matricula", cita.Matricula);
        command.Parameters.AddWithValue("@FechaInspeccion", cita.FechaInspeccion.ToString("o"));
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
                     SELECT * FROM Citas c
                     INNER JOIN Vehiculos v ON c.Matricula = v.Matricula
                     WHERE c.IsDeleted = 0
                     """;
        
        using var command = conexion.CreateCommand();

        if(!string.IsNullOrWhiteSpace(matricula))
        {
            sql += " AND c.Matricula = @Matricula";
            command.Parameters.AddWithValue("@Matricula", matricula);
        }
        
        if(!string.IsNullOrWhiteSpace(dni))
        {
            sql += " AND c.Dni = @Dni";
            command.Parameters.AddWithValue("@Dni", dni);
        }
        
        if(!string.IsNullOrWhiteSpace(marca))
        {
            sql += " AND v.Marca = @Marca";
            command.Parameters.AddWithValue("@Marca", marca);
        }
        
        if(!string.IsNullOrWhiteSpace(modelo))
        {
            sql += " AND v.Modelo = @Modelo";
            command.Parameters.AddWithValue("@Modelo", modelo);
        }

        if (tipoMotor.HasValue)
        {
            sql += " AND v.Motor = @Motor";
            command.Parameters.AddWithValue("@Motor", (int)tipoMotor.Value);
        }

        if (fechaPrincipio.HasValue && !fechaFinal.HasValue)
        {
            sql += " AND fechaInspeccion >= @FechaPrincipio";
            command.Parameters.AddWithValue("@FechaPrincipio", fechaPrincipio.Value.ToString("o"));
        } 
        else if (fechaPrincipio.HasValue && fechaFinal.HasValue)
        {
            sql += " AND fechaInspeccion BETWEEN @FechaPrincipio AND @FechaFinal";
            command.Parameters.AddWithValue("@FechaPrincipio", fechaPrincipio.Value.ToString("o"));
            command.Parameters.AddWithValue("@FechaFinal", fechaFinal.Value.ToString("o"));
        }
        
        sql += " ORDER BY c.FechaInspeccion LIMIT @Tamaño OFFSET @Paginas;";
        command.Parameters.AddWithValue("@Tamaño", tamañoPagina);
        command.Parameters.AddWithValue("@Paginas", (pagina - 1) * tamañoPagina);
        
        command.CommandText = sql;
        var output = new  List<Cita>();
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            output.Add(MapCita(reader));
        }

        foreach (var item in output)
        {
            item.Vehiculo = vehiculoRepo.GetByMatricula(item.Matricula)!;
        }
        
        return output;
    }

    private static Cita MapCita(SqliteDataReader reader)
    {
        return new Cita
        {
            Id = reader.GetInt32(0),
            Dni = reader.GetString(1),
            Matricula = reader.GetString(2),
            FechaInspeccion = DateTime.Parse(reader.GetString(3)),
            IsDeleted = reader.GetInt32(4) == 1,
            CreatedAt = DateTime.Parse(reader.GetString(5)),
            UpdatedAt = reader.IsDBNull(6) ? null : DateTime.Parse(reader.GetString(6))
        };
    }
}