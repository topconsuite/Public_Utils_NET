using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Commands;
using Telluria.Utils.Crud.Commands.BaseCommands;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Errors;
using Telluria.Utils.Crud.Handlers;
using Telluria.Utils.Crud.QueryFilters;
using Telluria.Utils.Crud.Repositories;
using Telluria.Utils.Crud.Validation;

namespace Telluria.Utils.Crud.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class BaseCrudController<TEntity, TValidator, TRepository, TCommandHandler> : ControllerBase
  where TEntity : BaseEntity
  where TValidator : BaseEntityValidator<TEntity>, new()
  where TRepository : IBaseCrudRepository<TEntity>
  where TCommandHandler : IBaseCrudCommandHandler<TEntity, TValidator, TRepository>
{
  protected bool IsNotFoundResult(ICommandResult result)
  {
    return result.Status != ECommandResultStatus.SUCCESS && result.ErrorCode == EClientError.NOT_FOUND.ToString();
  }

  protected bool IsFailedResult(ICommandResult result)
  {
    return result.Status != ECommandResultStatus.SUCCESS;
  }

  [HttpGet]
  public virtual async Task<IActionResult> List(
    [FromServices] TCommandHandler handler,
    [FromQuery] PagedRequestQuery<TEntity> query = null,
    CancellationToken cancellationToken = default)
  {
    IListCommand command = query!.Sort != null
      ? new BaseListSortedCommand<TEntity>(
        query.Page, query.PerPage,
        query.GetFilter(query.CaseSensitive),
        query.GetSorters(),
        query.GetIncludes(),
        cancellationToken)
      : new BaseListCommand<TEntity>(
        query.Page, query.PerPage, query.GetFilter(query.CaseSensitive), query.GetIncludes(), cancellationToken);

    var result = command is BaseListSortedCommand<TEntity> sortedCommand
      ? await handler.HandleAsync(sortedCommand)
      : await handler.HandleAsync((BaseListCommand<TEntity>)command);

    return IsFailedResult(result) ? BadRequest(result) : Ok(result);
  }

  [HttpGet("all")]
  public virtual async Task<IActionResult> ListAll(
    [FromServices] TCommandHandler handler,
    [FromQuery] PagedRequestQuery<TEntity> query = null,
    CancellationToken cancellationToken = default)
  {
    IListCommand command = query!.Sort != null
      ? new BaseListAllSortedCommand<TEntity>(
        query.Page,
        query.PerPage,
        query.GetFilter(query.CaseSensitive),
        query.GetSorters(),
        query.GetIncludes(),
        cancellationToken)
      : new BaseListAllCommand<TEntity>(
        query.Page, query.PerPage, query.GetFilter(query.CaseSensitive), query.GetIncludes(), cancellationToken);

    var result = command is BaseListAllSortedCommand<TEntity> sortedCommand
      ? await handler.HandleAsync(sortedCommand)
      : await handler.HandleAsync((BaseListAllCommand<TEntity>)command);

    return IsFailedResult(result) ? BadRequest(result) : Ok(result);
  }

  [HttpGet("find")]
  public virtual async Task<IActionResult> Find(
    [FromServices] TCommandHandler handler,
    [FromQuery] WhereRequestQuery<TEntity> query = null,
    CancellationToken cancellationToken = default)
  {
    var command = new BaseFindCommand<TEntity>(query.GetFilter(), query.GetIncludes(), cancellationToken);
    var result = await handler.HandleAsync(command);

    if (IsNotFoundResult(result)) return NotFound(result);
    if (IsFailedResult(result)) return BadRequest(result);

    return Ok(result);
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

    if (IsNotFoundResult(result)) return NotFound(result);
    if (IsFailedResult(result)) return BadRequest(result);

    return Ok(result);
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

    if (IsFailedResult(result)) return BadRequest(result);

    return Created($"{Request.Path}/{entity.Id}", result);
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

    if (IsFailedResult(result)) return BadRequest(result);

    var urls = entities.Select(t => $"{Request.Path}/{t.Id}");

    return Created(string.Join(", ", urls), result);
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

    if (IsFailedResult(result)) return BadRequest(result);

    return Ok(result);
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

    if (IsNotFoundResult(entity)) return NotFound(entity);

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

    if (IsFailedResult(result)) return BadRequest(result);

    return Ok(result);
  }

  [HttpDelete("{id}")]
  public virtual async Task<IActionResult> SoftDelete(
    [FromRoute] Guid id,
    [FromServices] TCommandHandler handler,
    CancellationToken cancellationToken = default)
  {
    var result = await handler.HandleAsync(new BaseSoftDeleteCommand(id, cancellationToken));

    if (IsNotFoundResult(result)) return NotFound(result);
    if (IsFailedResult(result)) return BadRequest(result);

    return Ok(result);
  }

  [HttpDelete("{id}/permanently")]
  public virtual async Task<IActionResult> Remove(
    [FromRoute] Guid id,
    [FromServices] TCommandHandler handler,
    CancellationToken cancellationToken = default)
  {
    var result = await handler.HandleAsync(new BaseRemoveCommand(id, cancellationToken));

    if (IsNotFoundResult(result)) return NotFound(result);
    if (IsFailedResult(result)) return BadRequest(result);

    return Ok(result);
  }
}
