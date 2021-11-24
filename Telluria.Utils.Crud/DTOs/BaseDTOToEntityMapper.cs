using Telluria.Utils.Crud.AutoMapper;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.DTOs
{
    public class BaseDTOToEntityMapper<TCreateDTO, TUpdateDTO, TEntity> : IDTOToEntityMapper<TCreateDTO, TUpdateDTO, TEntity>
        where TCreateDTO : BaseNotifiableRequestDTO<TEntity>
        where TUpdateDTO : BaseNotifiableRequestDTO<TEntity>
        where TEntity : BaseEntity
    {
        public virtual TEntity Map(TCreateDTO dto)
        {
            return Mapper.Map<TCreateDTO, TEntity>(dto);
        }

        public virtual TEntity Map(TUpdateDTO dto)
        {
            return Mapper.Map<TUpdateDTO, TEntity>(dto);
        }
    }
}
