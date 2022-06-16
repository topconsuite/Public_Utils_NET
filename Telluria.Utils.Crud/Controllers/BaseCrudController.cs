using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Commands.BaseCommands;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Handlers;
using Telluria.Utils.Crud.QueryFilters;
using Telluria.Utils.Crud.Repositories;
using Telluria.Utils.Crud.Validation;

namespace Telluria.Utils.Crud.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public abstract class BaseCrudController<TEntity, TValidator, TRepository, TCommandHandler> : ControllerBase
    where TEntity : BaseEntity
    where TValidator : BaseEntityValidator<TEntity>, new()
    where TRepository : IBaseCrudRepository<TEntity>
    where TCommandHandler : IBaseCrudCommandHandler<TEntity, TValidator, TRepository>
  {
    [HttpGet]
    public virtual async Task<IActionResult> List(
      [FromServices] TCommandHandler handler,
      [FromQuery] PagedRequestQuery<TEntity> query = null)
    {
      var command = new BaseListCommand<TEntity>(query.Page, query.PerPage, query.GetFilter(), query.GetIncludes());
      var result = await handler.HandleAsync(command);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpGet("all")]
    public virtual async Task<IActionResult> ListAll(
      [FromServices] TCommandHandler handler,
      [FromQuery] PagedRequestQuery<TEntity> query = null)
    {
      var command = new BaseListAllCommand<TEntity>(query.Page, query.PerPage, query.GetFilter(), query.GetIncludes());
      var result = await handler.HandleAsync(command);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpGet("find")]
    public virtual async Task<IActionResult> Find(
      [FromServices] TCommandHandler handler,
      [FromQuery] WhereRequestQuery<TEntity> query = null)
    {
      var command = new BaseFindCommand<TEntity>(query.GetFilter(), query.GetIncludes());
      var result = await handler.HandleAsync(command);
      if (result.Status == ECommandResultStatus.SUCCESS && result.Result == null) return NotFound(result);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> Get(
      Guid id,
      [FromServices] TCommandHandler handler,
      [FromQuery] IncludeRequestQuery query = null)
    {
      var command = new BaseGetCommand(id, query.GetIncludes());
      var result = await handler.HandleAsync(command);
      if (result.Status == ECommandResultStatus.SUCCESS && result.Result == null) return NotFound(result);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post(
      [FromServices] TCommandHandler handler,
      [FromBody] TEntity entity,
      [FromQuery] IncludeRequestQuery query = null)
    {
      var command = new BaseCreateCommand<TEntity>(entity, query.GetIncludes());
      var result = await handler.HandleAsync(command);
      return result.Status == ECommandResultStatus.SUCCESS ? Created($"{this.Request.Path}/{entity.Id}", result) : BadRequest(result);
    }

    [HttpPost("many")]
    public virtual async Task<IActionResult> PostMany(
      [FromServices] TCommandHandler handler,
      [FromQuery] IncludeRequestQuery query = null,
      [FromBody] params TEntity[] entities)
    {
      var command = new BaseCreateManyCommand<TEntity>(entities, query.GetIncludes());
      var result = await handler.HandleAsync(command);
      var urls = entities.Select(t => $"{Request.Path}/{t.Id}");
      return result.Status == ECommandResultStatus.SUCCESS ? Created(string.Join(", ", urls), result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Put(
      Guid id,
      [FromServices] TCommandHandler handler,
      [FromBody] TEntity entity,
      [FromQuery] IncludeRequestQuery query = null)
    {
      entity.Id = id;
      var command = new BaseUpdateCommand<TEntity>(entity, query.GetIncludes());
      var result = await handler.HandleAsync(command);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}")]
    public virtual async Task<IActionResult> Patch(
      Guid id,
      [FromServices] TCommandHandler handler,
      [FromBody] PatchBody<TEntity, TEntity> body,
      [FromQuery] IncludeRequestQuery query = null)
    {
      var entity = await handler.HandleAsync(new BaseGetCommand(id));
      if (entity.Status == ECommandResultStatus.SUCCESS && entity.Result == null) return NotFound(entity);

      try
      {
        body.ApplyTo(entity.Result);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }

      entity.Result.Id = id;
      var command = new BaseUpdateCommand<TEntity>(entity.Result, query.GetIncludes());
      var result = await handler.HandleAsync(command);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> SoftDelete(Guid id, [FromServices] TCommandHandler handler)
    {
      var result = await handler.HandleAsync(new BaseSoftDeleteCommand(id));
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}/permanently")]
    public virtual async Task<IActionResult> Remove(Guid id, [FromServices] TCommandHandler handler)
    {
      var result = await handler.HandleAsync(new BaseRemoveCommand(id));
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }
  }
}
