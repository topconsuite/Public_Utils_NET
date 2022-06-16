using System;
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
    protected void AddBaseMutationCreate<TGraphInputType>()
      where TGraphInputType : InputObjectGraphType<TEntity>
    {
      var entityName = typeof(TEntity).Name;

      Field<CommandResultType<TEntity, TGraphType>>()
        .Name("Create")
        .Argument<NonNullGraphType<TGraphInputType>>(entityName.ToCamelCase(), $"The {entityName} to create")
        .ResolveAsync(async context =>
        {
          var entity = context.GetArgument<TEntity>(entityName.ToCamelCase());
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
      var entityName = typeof(TEntity).Name;

      Field<CommandResultType<TEntity, TGraphType>>()
        .Name("Update")
        .Argument<NonNullGraphType<TGraphInputType>>(entityName.ToCamelCase(), $"The {entityName} to update")
        .ResolveAsync(async context =>
        {
          var entityDynamic = context.GetArgument<dynamic>(entityName.ToCamelCase());
          var includes = context.GetIncludes();
          var handler = context!.RequestServices!.GetRequiredService<TCommandHandler>();

          var getCommand = new BaseGetCommand(entityDynamic["id"]);
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
      var entityName = typeof(TEntity).Name;

      Field<CommandResultType>()
        .Name("Delete")
        .Argument<NonNullGraphType<TGraphType>>("id", $"The id of the {entityName} to delete")
        .Argument<BooleanGraphType>("permanent", $"If true, the {entityName} will be permanently deleted").DefaultValue(false)
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
