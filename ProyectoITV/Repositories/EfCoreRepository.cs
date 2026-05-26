using ProyectoITV.Entity;
using ProyectoITV.Enums;
using ProyectoITV.Models;

namespace ProyectoITV.Repositories;

public class EfCoreCitaRepository(AppDbContext context) : ICitaRepository
{
    const int tamañoPagina = 10;
    
    public Cita? GetById(int id)
    {
        var output =  context.Citas.Find(id);
        
        if (output == null) return null;
        
        return output;
    }

    public void Create(Cita cita)
    {
        context.Citas.Add(cita);
        context.SaveChanges();
    }

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

    public List<Cita> Consultar(string? matricula, string? dni, string? marca, string? modelo, TipoMotor? tipoMotor,
        DateTime? fechaPrincipio, DateTime? fechaFinal, int pagina)
    {
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
            Skip((pagina-1) * tamañoPagina). //Salta las paginas anteriores a la actual (1 para usuario 0 para programa), si entran 10 registros por pagina, salta pagina * 10 registros
            Take(tamañoPagina).ToList();
    }
}