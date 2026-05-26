using System.Data;
using Dapper;
using ProyectoITV.Enums;
using ProyectoITV.Models;

namespace ProyectoITV.Repositories;

public class DapperCitaRepository(IDbConnection conexion) : ICitaRepository
{
    const int tamañoPagina = 10;

    public Cita? GetById(int id)
    {
        string sql = "SELECT * FROM Citas WHERE Id = @Id";
        var output = conexion.Query<Cita>(sql, new { Id = id }).FirstOrDefault();

        if (output == null) return null;

        return output;
    }

    public void Create(Cita cita)
    {
        string sql = """
                     INSERT INTO Citas (Dni, Matricula, FechaInspeccion,  Marca, Modelo, Motor, FechaMatriculacion)
                     VALUES (@Dni, @Matricula, @FechaInspeccion, @Marca, @Modelo, @Motor, @FechaMatriculacion);
                     """;
        conexion.Execute(sql, cita);
    }

    public void Update(Cita cita, int id)
    {
        string sql = """
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

        conexion.Execute(sql, new
        {
            Dni = cita.Dni,
            Matricula = cita.Matricula,
            FechaInspeccion = cita.FechaInspeccion,
            Marca = cita.Marca,
            Modelo = cita.Modelo,
            Motor = (int)cita.Motor,
            FechaMatriculacion = cita.FechaMatriculacion.ToString("o"),
            IsDeleted = cita.IsDeleted,
            UpdatedAt = DateTime.Now.ToString("o"),
            Id = id
        });
    }

    public void Delete(int id, bool borradoLogico)
    {
        if (borradoLogico)
        {
            string sql = """
                         UPDATE Citas SET
                         IsDeleted = 1,
                         UpdatedAt = datetime('now')
                         WHERE Id = @Id;
                         """;
            conexion.Execute(sql, new { Id = id });
        }
        else
        {
            string sql = "DELETE FROM Citas WHERE Id = @Id";
            conexion.Execute(sql, new { Id = id });
        }
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

        int? motor = 0;

        if (tipoMotor.HasValue)
        {
            motor = (int)tipoMotor;
        }

        var output = conexion.Query<Cita>(sql, new
        {
            Matricula = string.IsNullOrWhiteSpace(matricula) ? null : matricula,
            Dni = string.IsNullOrWhiteSpace(dni) ? null : dni,
            Marca = string.IsNullOrWhiteSpace(marca) ? null : marca,
            Modelo = string.IsNullOrWhiteSpace(modelo) ? null : modelo,
            Motor = tipoMotor.HasValue ? motor : null,
            FechaPrincipio = fechaPrincipio.HasValue ? fechaPrincipio?.ToString("o") : null,
            FechaFinal = fechaFinal.HasValue ? fechaFinal?.ToString("o") : null,
            Tamaño = tamañoPagina,
            Paginas = (pagina - 1) * tamañoPagina
        }).ToList();

        return output;
    }
}