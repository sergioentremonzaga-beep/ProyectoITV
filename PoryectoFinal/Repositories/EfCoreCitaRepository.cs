using Microsoft.EntityFrameworkCore;
using PoryectoFinal.Data;
using PoryectoFinal.Interfaces;
using PoryectoFinal.Models;

namespace PoryectoFinal.Repositories;


public class EfCoreCitaRepository(AppDbContext context) : ICitaRepository
{
    const int tamañoPagina = 10;
    
    public Cita? GetById(int id)
    {
        return context.Citas.Find(id);
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
        IQueryable<Cita> consulta = context.Citas.Include(c => c.Vehiculo).Where(c => !c.IsDeleted); //Consulta base para no mostrar los borrados, include hace join
        
        if(!string.IsNullOrWhiteSpace(matricula)) //Las consultas se iran acumulando si cumplen los filtros, en vez de mandar diferentes listas
        {
            consulta = consulta.Where(c => c.Matricula == matricula);
        }
        
        if(!string.IsNullOrWhiteSpace(dni))
        {
            consulta = consulta.Where(c => c.Dni ==dni);
        }
        
        if(!string.IsNullOrWhiteSpace(marca))
        {
            consulta = consulta.Where(c => c.Vehiculo.Marca == marca);
        }
        
        if(!string.IsNullOrWhiteSpace(modelo))
        {
            consulta = consulta.Where(c => c.Vehiculo.Modelo == modelo);
        }

        if (tipoMotor.HasValue)
        {
            consulta = consulta.Where(c => c.Vehiculo.Motor == tipoMotor);
        }

        if (fechaPrincipio.HasValue && !fechaFinal.HasValue)
        {
            consulta = consulta.Where(c => c.FechaInspeccion >= fechaPrincipio);
        } 
        else if (fechaPrincipio.HasValue && fechaFinal.HasValue)
        {
            consulta = consulta.Where(c => c.FechaInspeccion >= fechaPrincipio.Value && c.FechaInspeccion <= fechaFinal.Value);
        }
        
        return consulta.OrderBy(c => c.FechaInspeccion).
            Skip((pagina-1) * tamañoPagina) //Salta las paginas anteriores a la actual (1 para usuario 0 para programa), si entran 10 registros por pagina, salta pagina * 10 registros
            .Take(tamañoPagina).ToList();
    }
}