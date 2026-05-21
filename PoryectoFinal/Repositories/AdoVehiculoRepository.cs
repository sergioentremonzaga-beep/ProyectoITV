using Microsoft.Data.Sqlite;
using PoryectoFinal.Interfaces;
using PoryectoFinal.Models;

namespace PoryectoFinal.Repositories;

public class AdoVehiculoRepository(SqliteConnection conexion) : IVehiculoRepository
{
    public Vehiculo? GetByMatricula(string matricula)
    {
        using var command = conexion.CreateCommand();
        command.CommandText = "SELECT * FROM Vehiculos WHERE Matricula = @Matricula";
        command.Parameters.AddWithValue("@Matricula", matricula);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapVehiculo(reader) :  null;
    }

    public void Create(Vehiculo vehiculo)
    {
        using var command = conexion.CreateCommand();
        command.CommandText = """
                              INSERT INTO Vehiculos (Matricula, Marca, Modelo, Motor, FechaMatriculacion)
                              VALUES (@Matricula, @Marca, @Modelo, @Motor, @FechaMatriculacion)
                              """;
        command.Parameters.AddWithValue("@Matricula", vehiculo.Matricula);
        command.Parameters.AddWithValue("@Marca", vehiculo.Marca);
        command.Parameters.AddWithValue("@Modelo", vehiculo.Modelo);
        command.Parameters.AddWithValue("@Motor", (int)vehiculo.Motor);
        command.Parameters.AddWithValue("@FechaMatriculacion", vehiculo.FechaMatriculacion.ToString("yyyy-MM-dd"));
        
        command.ExecuteNonQuery();
    }

    public void Update(Vehiculo vehiculo, string matricula)
    {
        using var command = conexion.CreateCommand();
        command.CommandText = """
                              UPDATE Vehiculos SET
                              Matricula = @Matricula,
                              Marca = @Marca,
                              Modelo = @Modelo,
                              Motor = @Motor,
                              FechaMatriculacion = @FechaMatriculacion,
                              UpdatedAt = @UpdatedAt
                              WHERE Matricula = @XMatricula;
                              """;
        
        command.Parameters.AddWithValue("@Matricula", vehiculo.Matricula);
        command.Parameters.AddWithValue("@XMatricula", matricula);
        command.Parameters.AddWithValue("@Marca", vehiculo.Marca);
        command.Parameters.AddWithValue("@Modelo", vehiculo.Modelo);
        command.Parameters.AddWithValue("@Motor", (int)vehiculo.Motor);
        command.Parameters.AddWithValue("@FechaMatriculacion", vehiculo.FechaMatriculacion.ToString("o"));
        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("o"));
        
        command.ExecuteNonQuery();
    }

    private static Vehiculo MapVehiculo(SqliteDataReader reader)
    {
        var motor = (TipoMotor)reader.GetInt32(3);

        return new Vehiculo
        {
            Matricula = reader.GetString(0),
            Marca = reader.GetString(1),
            Modelo = reader.GetString(2),
            Motor = motor,
            FechaMatriculacion = DateTime.Parse(reader.GetString(4)),
            IsDeleted = reader.GetInt32(5) == 1,
            CreatedAt = DateTime.Parse(reader.GetString(6)),
            UpdatedAt = reader.IsDBNull(7) ? null : DateTime.Parse(reader.GetString(7))
        };
    }
}