using System.Data;
using System.Diagnostics;
using Dapper;
using PoryectoFinal.Interfaces;
using PoryectoFinal.Models;

namespace PoryectoFinal.Repositories;

public class DapperCitaRepository(IDbConnection conexion, IVehiculoRepository vehiculoRepo) : ICitaRepository
{
    const int tamañoPagina = 10;
    
    public Cita? GetById(int id)
    {
        string sql = "SELECT * FROM Citas WHERE Id = @Id";
        var output = conexion.Query<Cita>(sql, new { Id = id }).FirstOrDefault();
        
        if (output == null) return null;
        
        output.Vehiculo = vehiculoRepo.GetByMatricula(output.Matricula);
        return output;
    }

    public void Create(Cita cita)
    {
        string sql = """
                     INSERT INTO Citas (Dni, Matricula, FechaInspeccion)
                     VALUES (@Dni, @Matricula, @FechaInspeccion);
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
                     IsDeleted = @IsDeleted,
                     UpdatedAt = @UpdatedAt
                     WHERE Id = @Id;
                     """;
        
        conexion.Execute(sql, new {
            Dni = cita.Dni,
            Matricula = cita.Matricula,
            FechaInspeccion = cita.FechaInspeccion,
            IsDeleted = cita.IsDeleted,
            UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
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
                     SELECT * FROM Citas c
                     INNER JOIN Vehiculos v ON c.Matricula = v.Matricula
                     WHERE c.IsDeleted = 0
                     """;
        
        if(!string.IsNullOrWhiteSpace(matricula))
        {
            sql += " AND c.Matricula = @Matricula";
        }
        
        if(!string.IsNullOrWhiteSpace(dni))
        {
            sql += " AND c.Dni = @Dni";
        }
        
        if(!string.IsNullOrWhiteSpace(marca))
        {
            sql += " AND v.Marca = @Marca";
        }
        
        if(!string.IsNullOrWhiteSpace(modelo))
        {
            sql += " AND v.Modelo = @Modelo";
        }

        if (tipoMotor.HasValue)
        {
            sql += " AND v.Motor = @Motor";
        }

        if (fechaPrincipio.HasValue && !fechaFinal.HasValue)
        {
            sql += " AND fechaInspeccion >= @FechaPrincipio";
        } 
        else if (fechaPrincipio.HasValue && fechaFinal.HasValue)
        {
            sql += " AND fechaInspeccion BETWEEN @FechaPrincipio AND @FechaFinal";
        }
        
        sql += " ORDER BY c.FechaInspeccion LIMIT @Tamaño OFFSET @Paginas;";
        
        int motor = 0;

        if (tipoMotor.HasValue)
        {
            motor = (int)tipoMotor;
        }
        
        var output = conexion.Query<Cita>(sql, new {
            Matricula = matricula,
            Dni = dni,
            Marca = marca,
            Modelo = modelo,
            Motor = motor,
            FechaPrincipio = fechaPrincipio,
            FechaFinal = fechaFinal,
            Tamaño = tamañoPagina,                         
            Paginas = (pagina - 1) * tamañoPagina
        }).ToList();

        foreach (var item in output)
        {
            item.Vehiculo = vehiculoRepo.GetByMatricula(item.Matricula);
        }
        
        return output;
    }
}