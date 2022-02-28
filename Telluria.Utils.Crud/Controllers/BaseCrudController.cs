using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Commands.BaseCommands;
using Telluria.Utils.Crud.DTOs;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Handlers;
using Telluria.Utils.Crud.QueryFilters;

namespace Telluria.Utils.Crud.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public abstract class BaseCrudController<TEntity, TCreateDTO, TUpdateDTO, TResponseDTO, TEntityToDTOMapper, TDTOToEntityMapper> : ControllerBase
      where TEntity : BaseEntity
      where TCreateDTO : BaseNotifiableRequestDTO<TEntity>
      where TUpdateDTO : BaseNotifiableRequestDTO<TEntity>
      where TResponseDTO : IResponseDTO<TEntity>
      where TEntityToDTOMapper : IEntityToDTOMapper<TEntity, TResponseDTO>, new()
      where TDTOToEntityMapper : IDTOToEntityMapper<TCreateDTO, TUpdateDTO, TEntity>, new()
  {
    private readonly IEntityToDTOMapper<TEntity, TResponseDTO> _entityToDTOMapper = new TEntityToDTOMapper();
    private readonly IDTOToEntityMapper<TCreateDTO, TUpdateDTO, TEntity> _dtoToEntityMapper = new TDTOToEntityMapper();

    [HttpGet]
    public virtual async Task<IActionResult> List([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
    {
      var result = await handler.HandleAsync(new BaseListCommand<TEntity>(query.Page, query.PerPage, query.GetFilter(), query.GetIncludes()));
      return result.Status == CommandResultStatus.SUCCESS ? Ok(_entityToDTOMapper.Map(result)) : BadRequest(result);
    }

    [HttpGet("all")]
    public virtual async Task<IActionResult> ListAll([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
    {
      var result = await handler.HandleAsync(new BaseListAllCommand<TEntity>(query.Page, query.PerPage, query.GetFilter(), query.GetIncludes()));
      return result.Status == CommandResultStatus.SUCCESS ? Ok(_entityToDTOMapper.Map(result)) : BadRequest(result);
    }

    [HttpGet("find")]
    public virtual async Task<IActionResult> Find([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
    {
      var result = await handler.HandleAsync(new BaseFindCommand<TEntity>(query.GetFilter(), query.GetIncludes()));
      if (result.Status == CommandResultStatus.SUCCESS && result.Result == null) return NotFound(result);
      return result.Status == CommandResultStatus.SUCCESS ? Ok(_entityToDTOMapper.Map(result)) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> Get([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id, [FromQuery] EntityRequestQuery<TEntity> query = null)
    {
      var result = await handler.HandleAsync(new BaseGetCommand<TEntity>(id, query.GetIncludes()));
      if (result.Status == CommandResultStatus.SUCCESS && result.Result == null) return NotFound(result);
      return result.Status == CommandResultStatus.SUCCESS ? Ok(_entityToDTOMapper.Map(result)) : BadRequest(result);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromBody] TCreateDTO payload)
    {
      if (!payload.IsValid)
        return BadRequest(new CommandResult(CommandResultStatus.ALERT, "Invalid body", null, payload.Notifications));

      var entity = _dtoToEntityMapper.Map(payload);
      var result = await handler.HandleAsync(new BaseCreateCommand<TEntity>(entity));
      return result.Status == CommandResultStatus.SUCCESS ? Created($"{this.Request.Path}/{entity.Id}", _entityToDTOMapper.Map(result)) : BadRequest(result);
    }

    [HttpPost("many")]
    public virtual async Task<IActionResult> PostMany([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromBody] params TCreateDTO[] payload)
    {
      var invalidList = payload.Where(t => !t.IsValid);
      if (invalidList.Count() > 0)
        return BadRequest(new CommandResult(CommandResultStatus.ALERT, "Invalid body", null, invalidList.Select(t => t.Notifications).First()));

      var entities = payload.Select(t => _dtoToEntityMapper.Map(t)).ToArray();
      var result = await handler.HandleAsync(new BaseCreateManyCommand<TEntity>(entities));
      var urls = entities.Select(t => $"{Request.Path}/{t.Id}");
      return result.Status == CommandResultStatus.SUCCESS ? Created(string.Join(", ", urls), _entityToDTOMapper.Map(result)) : BadRequest(result);
    }

    [HttpPatch("{id}")]
    public virtual async Task<IActionResult> Patch([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id, [FromBody] TUpdateDTO payload)
    {
      if (!payload.IsValid)
        return BadRequest(new CommandResult(CommandResultStatus.ALERT, "Invalid body", null, payload.Notifications));

      var entity = _dtoToEntityMapper.Map(payload);
      entity.Id = id;
      var result = await handler.HandleAsync(new BaseUpdateCommand<TEntity>(entity));
      return result.Status == CommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> SoftDelete([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id)
    {
      var result = await handler.HandleAsync(new BaseSoftDeleteCommand<TEntity>(id));
      return result.Status == CommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}/permanently")]
    public virtual async Task<IActionResult> Remove([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id)
    {
      var result = await handler.HandleAsync(new BaseRemoveCommand<TEntity>(id));
      return result.Status == CommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }
  }

  public abstract class BaseCrudController<TEntity, TCreateDTO, TUpdateDTO, TResponseDTO>
      : BaseCrudController<
          TEntity, TCreateDTO, TUpdateDTO, TResponseDTO,
          BaseEntityToDTOMapper<TEntity, TResponseDTO>,
          BaseDTOToEntityMapper<TCreateDTO, TUpdateDTO, TEntity>
      >
      where TEntity : BaseEntity
      where TCreateDTO : BaseNotifiableRequestDTO<TEntity>
      where TUpdateDTO : BaseNotifiableRequestDTO<TEntity>
      where TResponseDTO : IResponseDTO<TEntity>
  {
  }

  [ApiController]
  [Route("[controller]")]
  public abstract class BaseCrudController<TEntity> : ControllerBase where TEntity : BaseEntity
  {
    [HttpGet]
    public virtual async Task<IActionResult> List([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
    {
      var result = await handler.HandleAsync(new BaseListCommand<TEntity>(query.Page, query.PerPage, query.GetFilter(), query.GetIncludes()));
      return result.Status == CommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpGet("all")]
    public virtual async Task<IActionResult> ListAll([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
    {
      var result = await handler.HandleAsync(new BaseListAllCommand<TEntity>(query.Page, query.PerPage, query.GetFilter(), query.GetIncludes()));
      return result.Status == CommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpGet("find")]
    public virtual async Task<IActionResult> Find([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
    {
      var result = await handler.HandleAsync(new BaseFindCommand<TEntity>(query.GetFilter(), query.GetIncludes()));
      if (result.Status == CommandResultStatus.SUCCESS && result.Result == null) return NotFound(result);
      return result.Status == CommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> Get([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id, [FromQuery] EntityRequestQuery<TEntity> query = null)
    {
      var result = await handler.HandleAsync(new BaseGetCommand<TEntity>(id, query.GetIncludes()));
      if (result.Status == CommandResultStatus.SUCCESS && result.Result == null) return NotFound(result);
      return result.Status == CommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromBody] TEntity entity)
    {
      var result = await handler.HandleAsync(new BaseCreateCommand<TEntity>(entity));
      return result.Status == CommandResultStatus.SUCCESS ? Created($"{this.Request.Path}/{entity.Id}", result) : BadRequest(result);
    }

    [HttpPost("many")]
    public virtual async Task<IActionResult> PostMany([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromBody] params TEntity[] entities)
    {
      var result = await handler.HandleAsync(new BaseCreateManyCommand<TEntity>(entities));
      var urls = entities.Select(t => $"{Request.Path}/{t.Id}");
      return result.Status == CommandResultStatus.SUCCESS ? Created(string.Join(", ", urls), result) : BadRequest(result);
    }

    [HttpPatch("{id}")]
    public virtual async Task<IActionResult> Patch([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id, [FromBody] TEntity entity)
    {
      entity.Id = id;
      var result = await handler.HandleAsync(new BaseUpdateCommand<TEntity>(entity));
      return result.Status == CommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> SoftDelete([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id)
    {
      var result = await handler.HandleAsync(new BaseSoftDeleteCommand<TEntity>(id));
      return result.Status == CommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}/permanently")]
    public virtual async Task<IActionResult> Remove([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id)
    {
      var result = await handler.HandleAsync(new BaseRemoveCommand<TEntity>(id));
      return result.Status == CommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }
  }
}
