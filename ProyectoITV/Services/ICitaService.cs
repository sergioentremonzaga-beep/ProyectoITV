using ProyectoITV.Enums;
using ProyectoITV.Models;

namespace ProyectoITV.Services;

/// <summary>
/// Define el contrato de service
/// </summary>
public interface ICitaService
{
    /// <summary>
    /// Devuelve una cita específica por su ID
    /// </summary>
    /// <param name="id">El ID único de la cita</param>
    /// <returns>La Cita si existe, o null en caso contrario</returns>
    public Cita? GetById(int id);
    
    
    /// <summary>
    /// Inserta una nueva cita
    /// </summary>
    /// <param name="cita">La Cita a insertar</param>
    public void Create(Cita cita);
    
        
    /// <summary>
    /// Actualiza los datos de una cita
    /// </summary>
    /// <param name="cita">La Cita con los nuevos datos</param>
    /// <param name="id">El ID de la cita que se va a modificar</param>
    public void Update(Cita cita, int id);
    
    /// <summary>
    /// Elimina una cita de forma física o lógica
    /// </summary>
    /// <param name="id">El ID de la cita a eliminar</param>
    /// <param name="borradoLogico">Si es true, cambia IsDeleted a 1 (borrado), si es false, elimina completamente</param>
    public void Delete(int id, bool borradoLogico);
    
    /// <summary>
    /// Realiza una consulta filtrada y con paginación de los resultados
    /// </summary>
    /// <param name="matricula">Filtro por matrícula del vehículo</param>
    /// <param name="dni">Filtro por DNI</param>
    /// <param name="marca">Filtro por marca del vehículo</param>
    /// <param name="modelo">Filtro por modelo del vehículo</param>
    /// <param name="tipoMotor">Filtro por tipo de motor</param>
    /// <param name="fechaPrincipio">Fecha inicial del rango de inspección</param>
    /// <param name="fechaFinal">Fecha final del rango de inspección</param>
    /// <param name="pagina">El número de página para la paginación</param>
    /// <returns>Una lista de citas que coinciden con la consulta</returns>
    public List<Cita> Consultar(string? matricula, string? dni, string? marca, string? modelo, TipoMotor? tipoMotor, DateTime? fechaPrincipio, DateTime? fechaFinal, int pagina);
}