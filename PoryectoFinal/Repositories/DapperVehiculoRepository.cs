using System.Data;
using Dapper;
using PoryectoFinal.Data;
using PoryectoFinal.Interfaces;
using PoryectoFinal.Models;

namespace PoryectoFinal.Repositories;

public class DapperVehiculoRepository(IDbConnection conexion) : IVehiculoRepository
{
    public Vehiculo? GetByMatricula(string matricula)
    {
        string sql = "SELECT * FROM Vehiculos WHERE Matricula = @Matricula";
        
        return conexion.Query<Vehiculo>(sql, new { Matricula = matricula }).FirstOrDefault();
    }

    public void Create(Vehiculo vehiculo)
    {
        string sql = """
                     INSERT INTO Vehiculos (Matricula, Marca, Modelo, Motor, FechaMatriculacion)
                     VALUES (@Matricula, @Marca, @Modelo, @Motor, @FechaMatriculacion)
                     """;
        conexion.Execute(sql, vehiculo);
    }
    
    public void Update(Vehiculo vehiculo, string matricula)
    {
        vehiculo.UpdatedAt = DateTime.Now;

        string sql = """
                     UPDATE Vehiculos SET
                     Matricula = @Matricula,
                     Marca = @Marca,
                     Modelo = @Modelo,
                     Motor = @Motor,
                     FechaMatriculacion = @FechaMatriculacion,
                     UpdatedAt = @UpdatedAt
                     WHERE Matricula = @XMatricula;
                     """;
        
        conexion.Execute(sql, new {
            Matricula = vehiculo.Matricula,
            Marca = vehiculo.Marca,
            Modelo =  vehiculo.Modelo,
            Motor = (int)vehiculo.Motor,
            FechaMatriculacion = vehiculo.FechaMatriculacion.ToString("o"),
            UpdatedAt = DateTime.Now.ToString("o"),
            XMatricula = matricula
        });
    }
}