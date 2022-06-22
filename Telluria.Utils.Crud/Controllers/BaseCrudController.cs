using System;
using System.Linq;
using System.Threading;
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
      [FromQuery] PagedRequestQuery<TEntity> query = null,
      CancellationToken cancellationToken = default)
    {
      var command = new BaseListCommand<TEntity>(query.Page, query.PerPage, query.GetFilter(), query.GetIncludes(), cancellationToken);
      var result = await handler.HandleAsync(command);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpGet("all")]
    public virtual async Task<IActionResult> ListAll(
      [FromServices] TCommandHandler handler,
      [FromQuery] PagedRequestQuery<TEntity> query = null,
      CancellationToken cancellationToken = default)
    {
      var command = new BaseListAllCommand<TEntity>(query.Page, query.PerPage, query.GetFilter(), query.GetIncludes(), cancellationToken);
      var result = await handler.HandleAsync(command);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpGet("find")]
    public virtual async Task<IActionResult> Find(
      [FromServices] TCommandHandler handler,
      [FromQuery] WhereRequestQuery<TEntity> query = null,
      CancellationToken cancellationToken = default)
    {
      var command = new BaseFindCommand<TEntity>(query.GetFilter(), query.GetIncludes(), cancellationToken);
      var result = await handler.HandleAsync(command);
      if (result.Status == ECommandResultStatus.SUCCESS && result.Result == null) return NotFound(result);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> Get(
      [FromRoute] Guid id,
      [FromServices] TCommandHandler handler,
      [FromQuery] IncludeRequestQuery query = null,
      CancellationToken cancellationToken = default)
    {
      var command = new BaseGetCommand(id, query.GetIncludes(), cancellationToken);
      var result = await handler.HandleAsync(command);
      if (result.Status == ECommandResultStatus.SUCCESS && result.Result == null) return NotFound(result);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post(
      [FromBody] TEntity entity,
      [FromServices] TCommandHandler handler,
      [FromQuery] IncludeRequestQuery query = null,
      CancellationToken cancellationToken = default)
    {
      var command = new BaseCreateCommand<TEntity>(entity, query.GetIncludes(), cancellationToken);
      var result = await handler.HandleAsync(command);
      return result.Status == ECommandResultStatus.SUCCESS ? Created($"{this.Request.Path}/{entity.Id}", result) : BadRequest(result);
    }

    [HttpPost("many")]
    public virtual async Task<IActionResult> PostMany(
      [FromBody] TEntity[] entities,
      [FromServices] TCommandHandler handler,
      [FromQuery] IncludeRequestQuery query = null,
      CancellationToken cancellationToken = default)
    {
      var command = new BaseCreateManyCommand<TEntity>(entities, query.GetIncludes(), cancellationToken);
      var result = await handler.HandleAsync(command);
      var urls = entities.Select(t => $"{Request.Path}/{t.Id}");
      return result.Status == ECommandResultStatus.SUCCESS ? Created(string.Join(", ", urls), result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Put(
      [FromRoute] Guid id,
      [FromBody] TEntity entity,
      [FromServices] TCommandHandler handler,
      [FromQuery] IncludeRequestQuery query = null,
      CancellationToken cancellationToken = default)
    {
      entity.Id = id;
      var command = new BaseUpdateCommand<TEntity>(entity, query.GetIncludes(), cancellationToken);
      var result = await handler.HandleAsync(command);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}")]
    public virtual async Task<IActionResult> Patch(
      [FromRoute] Guid id,
      [FromBody] PatchBody<TEntity, TEntity> body,
      [FromServices] TCommandHandler handler,
      [FromQuery] IncludeRequestQuery query = null,
      CancellationToken cancellationToken = default)
    {
      var entity = await handler.HandleAsync(new BaseGetCommand(id, null, cancellationToken));
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
      var command = new BaseUpdateCommand<TEntity>(entity.Result, query.GetIncludes(), cancellationToken);
      var result = await handler.HandleAsync(command);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> SoftDelete(
      [FromRoute] Guid id,
      [FromServices] TCommandHandler handler,
      CancellationToken cancellationToken = default)
    {
      var result = await handler.HandleAsync(new BaseSoftDeleteCommand(id, cancellationToken));
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}/permanently")]
    public virtual async Task<IActionResult> Remove(
      [FromRoute] Guid id,
      [FromServices] TCommandHandler handler,
      CancellationToken cancellationToken = default)
    {
      var result = await handler.HandleAsync(new BaseRemoveCommand(id, cancellationToken));
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }
  }
}
