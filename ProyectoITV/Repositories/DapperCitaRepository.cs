using System.Data;
using Dapper;
using ProyectoITV.Enums;
using ProyectoITV.Models;

namespace ProyectoITV.Repositories;

/// <summary>
/// Esta clase gestiona el acceso a datos utilizando Dapper
/// </summary>
/// <param name="conexion">La conexión a la base de datos</param>
public class DapperCitaRepository(IDbConnection conexion) : ICitaRepository
{
    const int tamañoPagina = 10;

    /// <summary>
    /// Devuelve una cita específica por su ID
    /// </summary>
    public Cita? GetById(int id)
    {
        string sql = "SELECT * FROM Citas WHERE Id = @Id";
        var output = conexion.Query<Cita>(sql, new { Id = id }).FirstOrDefault();

        if (output == null) return null;

        return output;
    }

    /// <summary>
    /// Inserta una nueva cita en la base de datos
    /// </summary>
    public void Create(Cita cita)
    {
        string sql = """
                     INSERT INTO Citas (Dni, Matricula, FechaInspeccion,  Marca, Modelo, Motor, FechaMatriculacion)
                     VALUES (@Dni, @Matricula, @FechaInspeccion, @Marca, @Modelo, @Motor, @FechaMatriculacion);
                     """;
        conexion.Execute(sql, cita);
    }

    /// <summary>
    /// Actualiza los datos de una cita
    /// </summary>
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
    
    /// <summary>
    /// Elimina una cita de forma física o lógica
    /// </summary>
    /// <param name="borradoLogico">Si es true, cambia IsDeleted a 1 (borrado), si es false, elimina la cita de la base de datos</param>
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
            Paginas = (pagina - 1) * tamañoPagina // Toma 10 registros por pagina y salta todos los anteriores registros (pagina-1 * 10 = todos los registros a saltar == todas las anteriores páginas)
        }).ToList();
        
        return output;
    }
}