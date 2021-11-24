using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telluria.Utils.Crud.QueryFilters;
using Telluria.Utils.Crud.Handlers;
using Telluria.Utils.Crud.Commands.BaseCommands;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.DTOs;
using Telluria.Utils.Crud.CommandResults;

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
            return Ok(_entityToDTOMapper.Map(result));
        }

        [HttpGet("all")]
        public virtual async Task<IActionResult> ListAll([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
        {
            var result = await handler.HandleAsync(new BaseListAllCommand<TEntity>(query.Page, query.PerPage, query.GetFilter(), query.GetIncludes()));
            return Ok(_entityToDTOMapper.Map(result));
        }

        [HttpGet("find")]
        public virtual async Task<IActionResult> Find([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
        {
            var result = await handler.HandleAsync(new BaseFindCommand<TEntity>(query.GetFilter(), query.GetIncludes()));
            return Ok(_entityToDTOMapper.Map(result));
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id, [FromQuery] EntityRequestQuery<TEntity> query = null)
        {
            var result = await handler.HandleAsync(new BaseGetCommand<TEntity>(id, query.GetIncludes()));
            return Ok(_entityToDTOMapper.Map(result));
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromBody] TCreateDTO payload)
        {
            if (!payload.IsValid)
                return BadRequest(new CommandResult(false, "Invalid body", payload.Notifications));

            var entity = _dtoToEntityMapper.Map(payload);
            var result = await handler.HandleAsync(new BaseCreateCommand<TEntity>(entity));
            return Created($"{this.Request.Path}/{entity.Id}", _entityToDTOMapper.Map(result));
        }

        [HttpPost("many")]
        public virtual async Task<IActionResult> PostMany([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromBody] params TCreateDTO[] payload)
        {
            var invalidList = payload.Where(t => !t.IsValid);
            if (invalidList.Count() > 0)
                return BadRequest(new CommandResult(false, "Invalid body", invalidList.Select(t => t.Notifications).First()));

            var entities = payload.Select(t => _dtoToEntityMapper.Map(t)).ToArray();
            var result = await handler.HandleAsync(new BaseCreateManyCommand<TEntity>(entities));
            var urls = entities.Select(t => $"{Request.Path}/{t.Id}");
            return Created(string.Join(", ", urls), _entityToDTOMapper.Map(result));
        }

        [HttpPatch]
        public virtual async Task<IActionResult> Patch([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromBody] TUpdateDTO payload)
        {
            if (!payload.IsValid)
                return BadRequest(new CommandResult(false, "Invalid body", payload.Notifications));

            var entity = _dtoToEntityMapper.Map(payload);
            var result = await handler.HandleAsync(new BaseUpdateCommand<TEntity>(entity));
            return Ok(result);
        }

        [HttpPatch("many")]
        public virtual async Task<IActionResult> PatchMany([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromBody] params TUpdateDTO[] payload)
        {
            var invalidList = payload.Where(t => !t.IsValid);
            if (invalidList.Count() > 0)
                return BadRequest(new CommandResult(false, "Invalid body", invalidList.Select(t => t.Notifications).First()));

            var entities = payload.Select(t => _dtoToEntityMapper.Map(t)).ToArray();
            var result = await handler.HandleAsync(new BaseUpdateManyCommand<TEntity>(entities));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> SoftDelete([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id)
        {
            var result = await handler.HandleAsync(new BaseSoftDeleteCommand<TEntity>(id));
            return Ok(result);
        }

        [HttpDelete("{id}/permanently")]
        public virtual async Task<IActionResult> Remove([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id)
        {
            var result = await handler.HandleAsync(new BaseRemoveCommand<TEntity>(id));
            return Ok(result);
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
            return Ok(result);
        }

        [HttpGet("all")]
        public virtual async Task<IActionResult> ListAll([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
        {
            var result = await handler.HandleAsync(new BaseListAllCommand<TEntity>(query.Page, query.PerPage, query.GetFilter(), query.GetIncludes()));
            return Ok(result);
        }

        [HttpGet("find")]
        public virtual async Task<IActionResult> Find([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromQuery] EntityRequestQuery<TEntity> query = null)
        {
            var result = await handler.HandleAsync(new BaseFindCommand<TEntity>(query.GetFilter(), query.GetIncludes()));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id, [FromQuery] EntityRequestQuery<TEntity> query = null)
        {
            var result = await handler.HandleAsync(new BaseGetCommand<TEntity>(id, query.GetIncludes()));
            return Ok(result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromBody] TEntity entity)
        {
            var result = await handler.HandleAsync(new BaseCreateCommand<TEntity>(entity));
            return Created($"{this.Request.Path}/{entity.Id}", result);
        }

        [HttpPost("many")]
        public virtual async Task<IActionResult> PostMany([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromBody] params TEntity[] entities)
        {
            var result = await handler.HandleAsync(new BaseCreateManyCommand<TEntity>(entities));
            var urls = entities.Select(t => $"{Request.Path}/{t.Id}");
            return Created(string.Join(", ", urls), result);
        }

        [HttpPatch]
        public virtual async Task<IActionResult> Patch([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromBody] TEntity entity)
        {
            var result = await handler.HandleAsync(new BaseUpdateCommand<TEntity>(entity));
            return Ok(result);
        }

        [HttpPatch("many")]
        public virtual async Task<IActionResult> PatchMany([FromServices] IBaseCrudCommandHandler<TEntity> handler, [FromBody] params TEntity[] entities)
        {
            var result = await handler.HandleAsync(new BaseUpdateManyCommand<TEntity>(entities));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> SoftDelete([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id)
        {
            var result = await handler.HandleAsync(new BaseSoftDeleteCommand<TEntity>(id));
            return Ok(result);
        }

        [HttpDelete("{id}/permanently")]
        public virtual async Task<IActionResult> Remove([FromServices] IBaseCrudCommandHandler<TEntity> handler, Guid id)
        {
            var result = await handler.HandleAsync(new BaseRemoveCommand<TEntity>(id));
            return Ok(result);
        }
    }
}
