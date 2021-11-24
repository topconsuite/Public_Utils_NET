using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.DTOs
{
    public interface IDTOToEntityMapper<TCreateDTO, TUpdateDTO, TEntity>
        where TCreateDTO : BaseNotifiableRequestDTO<TEntity>
        where TUpdateDTO : BaseNotifiableRequestDTO<TEntity>
        where TEntity : BaseEntity
    {
        TEntity Map(TCreateDTO dto);
        TEntity Map(TUpdateDTO dto);
    }
}
