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
    public virtual async Task<IActionResult> List([FromServices] TCommandHandler handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
    {
      var result = await handler.HandleAsync(new BaseListCommand<TEntity>(query.Page, query.PerPage, query.GetFilter(), query.GetIncludes()));
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpGet("all")]
    public virtual async Task<IActionResult> ListAll([FromServices] TCommandHandler handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
    {
      var result = await handler.HandleAsync(new BaseListAllCommand<TEntity>(query.Page, query.PerPage, query.GetFilter(), query.GetIncludes()));
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpGet("find")]
    public virtual async Task<IActionResult> Find([FromServices] TCommandHandler handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
    {
      var result = await handler.HandleAsync(new BaseFindCommand<TEntity>(query.GetFilter(), query.GetIncludes()));
      if (result.Status == ECommandResultStatus.SUCCESS && result.Result == null) return NotFound(result);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> Get([FromServices] TCommandHandler handler, Guid id, [FromQuery] EntityRequestQuery<TEntity> query = null)
    {
      var result = await handler.HandleAsync(new BaseGetCommand(id, query.GetIncludes()));
      if (result.Status == ECommandResultStatus.SUCCESS && result.Result == null) return NotFound(result);
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post([FromServices] TCommandHandler handler, [FromBody] TEntity entity)
    {
      var result = await handler.HandleAsync(new BaseCreateCommand<TEntity>(entity));
      return result.Status == ECommandResultStatus.SUCCESS ? Created($"{this.Request.Path}/{entity.Id}", result) : BadRequest(result);
    }

    [HttpPost("many")]
    public virtual async Task<IActionResult> PostMany([FromServices] TCommandHandler handler, [FromBody] params TEntity[] entities)
    {
      var result = await handler.HandleAsync(new BaseCreateManyCommand<TEntity>(entities));
      var urls = entities.Select(t => $"{Request.Path}/{t.Id}");
      return result.Status == ECommandResultStatus.SUCCESS ? Created(string.Join(", ", urls), result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Put([FromServices] TCommandHandler handler, Guid id, [FromBody] TEntity entity)
    {
      entity.Id = id;
      var result = await handler.HandleAsync(new BaseUpdateCommand<TEntity>(entity));
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}")]
    public virtual async Task<IActionResult> Patch([FromServices] TCommandHandler handler, Guid id, [FromBody] PatchBody<TEntity, TEntity> body)
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
      var result = await handler.HandleAsync(new BaseUpdateCommand<TEntity>(entity.Result));
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> SoftDelete([FromServices] TCommandHandler handler, Guid id)
    {
      var result = await handler.HandleAsync(new BaseSoftDeleteCommand(id));
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}/permanently")]
    public virtual async Task<IActionResult> Remove([FromServices] TCommandHandler handler, Guid id)
    {
      var result = await handler.HandleAsync(new BaseRemoveCommand(id));
      return result.Status == ECommandResultStatus.SUCCESS ? Ok(result) : BadRequest(result);
    }
  }
}
