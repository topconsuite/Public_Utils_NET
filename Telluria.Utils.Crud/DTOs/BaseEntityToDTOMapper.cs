using System.Collections.Generic;
using System.Linq;
using Telluria.Utils.Crud.AutoMapper;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Lists;

namespace Telluria.Utils.Crud.DTOs
{
  public class BaseEntityToDTOMapper<TEntity, TDTO> : IEntityToDTOMapper<TEntity, TDTO>
      where TEntity : BaseEntity
      where TDTO : IResponseDTO<TEntity>
  {
    public virtual TDTO Map(TEntity entity)
    {
      return Mapper.Map<TEntity, TDTO>(entity);
    }

    public ICommandResult<TDTO> Map(ICommandResult<TEntity> commandResult)
    {
      return new CommandResult<TDTO>(commandResult.Status, commandResult.Message, Map(commandResult.Result), commandResult.Notifications);
    }

    public ICommandResult<IEnumerable<TDTO>> Map(ICommandResult<IEnumerable<TEntity>> commandResult)
    {
      return new CommandResult<IEnumerable<TDTO>>(
          commandResult.Status,
          commandResult.Message,
          commandResult.Result.Select(t => Map(t)),
          commandResult.Notifications
      );
    }

    public IListCommandResult<TDTO> Map(IListCommandResult<TEntity> commandResult)
    {
      return new ListCommandResult<TDTO>(
          commandResult.Status,
          commandResult.Message,
          new PagedList<TDTO>
          {
            Page = commandResult.Page,
            PerPage = commandResult.PerPage,
            PageCount = commandResult.PageCount,
            TotalCount = commandResult.TotalCount,
            Records = commandResult.Result.Select(t => Map(t)),
          },
          commandResult.Notifications
      );
    }
  }
}
