namespace ProyectoITV.Mappers;

/// <summary>
/// Define el contrato genérico para los mapeos de modelo a dto y a la inversa
/// </summary>
/// <typeparam name="T">El tipo de la entidad (clase)</typeparam>
/// <typeparam name="TDto">El tipo del DTO</typeparam>
public interface IMapper<T, TDto> where T : class
{
    /// <summary>
    /// Convierte un objeto de tipo TDto (objeto DTO) a T (objeto modelo)
    /// </summary>
    /// <param name="dto">El objeto DTO</param>
    /// <returns>Una instancia del objeto T (modelo)</returns>
    public T ToModel(TDto dto);
    
    /// <summary>
    /// Convierte un objeto de tipo T (objeto modelo) a TDto (objeto DTO)
    /// </summary>
    /// <param name="model">El objeto modelo</param>
    /// <returns>Una instancia del objeto TDto (DTO)</returns>
    public TDto ToDto(T model);
}