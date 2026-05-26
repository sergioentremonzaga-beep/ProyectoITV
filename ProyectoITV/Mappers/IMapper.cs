namespace ProyectoITV.Mappers;

public interface IMapper<T, TDto> where T : class
{
    public T ToModel(TDto dto);
    public TDto ToDto(T model);
}