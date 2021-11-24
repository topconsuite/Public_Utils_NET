using System.Collections.Generic;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.DTOs
{
    public interface IEntityToDTOMapper<TEntity, TDTO>
        where TEntity : BaseEntity
        where TDTO : IResponseDTO<TEntity>
    {
        TDTO Map(TEntity entity);
        ICommandResult<TDTO> Map(ICommandResult<TEntity> commandResult);
        ICommandResult<IEnumerable<TDTO>> Map(ICommandResult<IEnumerable<TEntity>> commandResult);
        IListCommandResult<TDTO> Map(IListCommandResult<TEntity> commandResult);
    }
}
