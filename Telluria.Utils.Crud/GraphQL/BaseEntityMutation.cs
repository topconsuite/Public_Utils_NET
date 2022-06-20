using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Commands.BaseCommands;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.GraphQL.Types;
using Telluria.Utils.Crud.Handlers;
using Telluria.Utils.Crud.Repositories;
using Telluria.Utils.Crud.Validation;

namespace Telluria.Utils.Crud.GraphQL
{
  public abstract class BaseEntityMutation<TEntity, TGraphType, TValidator, TRepository, TCommandHandler> : ObjectGraphType
    where TEntity : BaseEntity
    where TGraphType : BaseEntityGraphType<TEntity>
    where TValidator : BaseEntityValidator<TEntity>, new()
    where TRepository : IBaseCrudRepository<TEntity>
    where TCommandHandler : IBaseCrudCommandHandler<TEntity, TValidator, TRepository>
  {
    private readonly string _entityName;

    protected BaseEntityMutation()
    {
      _entityName = typeof(TEntity).Name;

      Name = $"{_entityName}Mutations";
      Description = $"Mutations for {_entityName}";
    }

    protected void AddBaseMutationCreate<TGraphInputType>()
      where TGraphInputType : BaseCreateInputType<TEntity>
    {
      Field<CommandResultType<TEntity, TGraphType>>()
        .Name("Create")
        .Argument<NonNullGraphType<TGraphInputType>>(_entityName.ToCamelCase(), $"The {_entityName} to create")
        .ResolveAsync(async context =>
        {
          var entity = context.GetArgument<TEntity>(_entityName.ToCamelCase());
          var includes = context.GetIncludes();
          var handler = context!.RequestServices!.GetRequiredService<TCommandHandler>();
          var command = new BaseCreateCommand<TEntity>(entity, includes);
          var response = await handler.HandleAsync(command);

          if (response.Status == ECommandResultStatus.SUCCESS)
            return response;

          throw GraphQLExecutionError.Create(response.Message, response.ErrorCode, response.Status);
        });
    }

    protected void AddBaseMutationUpdate<TGraphInputType>()
      where TGraphInputType : BaseUpdateInputType<TEntity>
    {
      Field<CommandResultType<TEntity, TGraphType>>()
        .Name("Update")
        .Argument<NonNullGraphType<TGraphInputType>>(_entityName.ToCamelCase(), $"The {_entityName} to update")
        .ResolveAsync(async context =>
        {
          var entityDynamic = context.GetArgument<Dictionary<string, object>>(_entityName.ToCamelCase());
          var includes = context.GetIncludes();
          var handler = context!.RequestServices!.GetRequiredService<TCommandHandler>();

          var id = new Guid(entityDynamic["id"].ToString());
          var getCommand = new BaseGetCommand(id);
          var oldResponse = await handler.HandleAsync(getCommand);
          var entityDb = oldResponse.Result;

          JsonConvert.PopulateObject(JsonConvert.SerializeObject(entityDynamic), entityDb);

          var command = new BaseUpdateCommand<TEntity>(entityDb, includes);
          var response = await handler.HandleAsync(command);

          if (response.Status == ECommandResultStatus.SUCCESS)
            return response;

          throw GraphQLExecutionError.Create(response.Message, response.ErrorCode, response.Status);
        });
    }

    protected void AddBaseMutationDelete()
    {
      Field<CommandResultType>()
        .Name("Delete")
        .Argument<NonNullGraphType<GuidGraphType>>("id", $"The id of the {_entityName} to delete")
        .Argument<BooleanGraphType>("permanent", $"If true, the {_entityName} will be permanently deleted").DefaultValue(false)
        .ResolveAsync(async context =>
        {
          var handler = context!.RequestServices!.GetRequiredService<TCommandHandler>();
          var id = context.GetArgument<Guid>("id");

          var response = context.GetArgument<bool>("permanent")
            ? await handler.HandleAsync(new BaseRemoveCommand(id))
            : await handler.HandleAsync(new BaseSoftDeleteCommand(id));

          if (response.Status == ECommandResultStatus.SUCCESS)
            return response;

          throw GraphQLExecutionError.Create(response.Message, response.ErrorCode, response.Status);
        });
    }
  }
}
