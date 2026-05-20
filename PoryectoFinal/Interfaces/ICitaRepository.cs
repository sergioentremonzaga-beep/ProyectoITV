using PoryectoFinal.Models;

namespace PoryectoFinal.Interfaces;

public interface ICitaRepository
{
    public Cita? GetById(int id);
    public void Create(Cita cita);
    public void Update(Cita cita, int id);
    public void Delete(int id, bool borradoLogico);
    public List<Cita> Consultar(string? matricula, string? dni, string? marca, string? modelo, TipoMotor? tipoMotor, DateTime? fechaPrincipio, DateTime? fechaFinal, int pagina);
}