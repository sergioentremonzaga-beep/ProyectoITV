using ProyectoITV.Entity;
using ProyectoITV.Enums;
using ProyectoITV.Models;

namespace ProyectoITV.Repositories;

/// <summary>
/// Esta clase gestiona el acceso a datos utilizando EFCore
/// </summary>
/// <param name="conexion">La conexión a la base de datos</param>
public class EfCoreCitaRepository(AppDbContext context) : ICitaRepository
{
    const int tamañoPagina = 10;
    
    /// <summary>
    /// Devuelve una cita específica por su ID
    /// </summary>
    public Cita? GetById(int id)
    {
        var output =  context.Citas.Find(id);
        
        if (output == null) return null;
        
        return output;
    }
    
    /// <summary>
    /// Inserta una nueva cita en la base de datos
    /// </summary>
    public void Create(Cita cita)
    {
        context.Citas.Add(cita);
        context.SaveChanges();
    }

    /// <summary>
    /// Actualiza los datos de una cita
    /// </summary>
    public void Update(Cita cita, int id)
    {
        var exists = context.Citas.Find(id);
        if (exists != null)
        {
            exists.UpdatedAt = DateTime.Now;
            exists.Dni = cita.Dni;
            exists.Matricula = cita.Matricula;
            exists.FechaInspeccion = cita.FechaInspeccion;
            exists.Marca = cita.Marca;
            exists.FechaMatriculacion = cita.FechaMatriculacion;
            exists.Modelo = cita.Modelo;
            exists.Motor = cita.Motor;
            
            context.SaveChanges();
        }
    }
    
    /// <summary>
    /// Elimina una cita de forma física o lógica
    /// </summary>
    /// <param name="borradoLogico">Si es true, cambia IsDeleted a 1 (borrado), si es false, elimina la cita de la base de datos</param>
    public void Delete(int id, bool borradoLogico)
    {
        var exists = context.Citas.Find(id);
        if (exists != null)
        {
            if (borradoLogico)
            {
                exists.IsDeleted = true;
                exists.UpdatedAt = DateTime.Now;
                
                context.SaveChanges();
            }
            else
            {
                context.Citas.Remove(exists);
                context.SaveChanges();
            }
        }
    }

    /// <summary>
    /// Realiza una consulta filtrada y con paginación de los resultados
    /// </summary>
    public List<Cita> Consultar(string? matricula, string? dni, string? marca, string? modelo, TipoMotor? tipoMotor,
        DateTime? fechaPrincipio, DateTime? fechaFinal, int pagina)
    {
        // Si el parámetro no es nulo lo toma e ignora el OR, en caso de que lo sea toma el nulo
        return context.Citas.
            Where(c => !c.IsDeleted 
                && ((c.Matricula == matricula) || (string.IsNullOrWhiteSpace(matricula)))
            && ((c.Dni == dni) || (string.IsNullOrWhiteSpace(dni)))
            && ((c.Marca == marca) || (string.IsNullOrWhiteSpace(marca)))
            && ((c.Modelo == modelo) || (string.IsNullOrWhiteSpace(modelo)))
            && ((c.Motor == tipoMotor) || (!tipoMotor.HasValue))
            && ((c.FechaInspeccion >= fechaPrincipio) || (!fechaPrincipio.HasValue))
            && ((c.FechaInspeccion <= fechaFinal) || (!fechaFinal.HasValue))).
            OrderBy(c => c.FechaInspeccion).
            Skip((pagina-1) * tamañoPagina). // Toma 10 registros por pagina y salta todos los anteriores registros (pagina-1 * 10 = todos los registros a saltar == todas las anteriores páginas)
            Take(tamañoPagina).ToList();
    }
}