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
using HL = Telluria.Utils.Crud.GraphQL.Helpers;

namespace Telluria.Utils.Crud.GraphQL.Mutations
{
  public class BaseEntityMutation<TEntity, TGraphType> : BaseEntityMutation<TEntity, TGraphType, IBaseCrudCommandHandler<TEntity>>
    where TEntity : BaseEntity
    where TGraphType : ObjectGraphType<TEntity>
  {
  }

  public class BaseEntityMutation<TEntity, TGraphType, TCommandHandler> : ObjectGraphType
    where TEntity : BaseEntity
    where TGraphType : ObjectGraphType<TEntity>
    where TCommandHandler : IBaseCrudCommandHandler<TEntity>
  {
    protected void AddBaseMutationCreate<TGraphInputType>()
      where TGraphInputType : InputObjectGraphType<TEntity>
    {
      var entityName = typeof(TEntity).Name;
      var entityNameCamalCase = char.ToLowerInvariant(entityName[0]) + entityName[1..];

      Field<CommandResultType<TEntity, TGraphType>>()
        .Name("Create")
        .Argument<NonNullGraphType<TGraphInputType>>(
          entityNameCamalCase, $"The {entityName} to create")
        .ResolveAsync(async context =>
        {
          var includes = new List<string>();
          var result = context?.SubFields?["result"];
          var selections = result?.SelectionSet?.Selections;

          // Get the includes (If has any)
          if (selections != null)
            HL.RecursiveIncludes.AddRecursiveIncludes(selections, includes);

          var handler = context!.RequestServices!.GetRequiredService<TCommandHandler>();
          var response = await handler.HandleAsync(
            new BaseCreateCommand<TEntity>(
              context.GetArgument<TEntity>(entityNameCamalCase), includes.ToArray())
          );

          if (response.Status == CommandResultStatus.SUCCESS)
            return response;

          throw HL.ExecutionError.Create(response.Message, response.ErrorCode, response.Status);
        });
    }

    protected void AddBaseMutationUpdate<TGraphInputType>()
      where TGraphInputType : InputObjectGraphType<TEntity>
    {
      var entityName = typeof(TEntity).Name;
      var entityNameCamalCase = char.ToLowerInvariant(entityName[0]) + entityName[1..];

      Field<CommandResultType<TEntity, TGraphType>>()
        .Name("Update")
        .Argument<NonNullGraphType<TGraphInputType>>(
          entityNameCamalCase, $"The {entityName} to update")
        .ResolveAsync(async context =>
        {
          var includes = new List<string>();
          var result = context?.SubFields?["result"];
          var selections = result?.SelectionSet?.Selections;

          // Get the includes (If has any)
          if (selections != null)
            HL.RecursiveIncludes.AddRecursiveIncludes(selections, includes);

          var handler = context!.RequestServices!.GetRequiredService<TCommandHandler>();
          var entityDynamic = context.GetArgument<dynamic>(entityNameCamalCase);
          var entityDb = handler.HandleAsync(new BaseGetCommand<TEntity>(entityDynamic["id"])).Result.Result;

          JsonConvert.PopulateObject(JsonConvert.SerializeObject(entityDynamic), entityDb);

          var response = await handler.HandleAsync(
            new BaseUpdateCommand<TEntity>(entityDb, includes.ToArray())
          );

          if (response.Status == CommandResultStatus.SUCCESS)
            return response;

          throw HL.ExecutionError.Create(response.Message, response.ErrorCode, response.Status);
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
            ? await handler.HandleAsync(new BaseRemoveCommand<TEntity>(id))
            : await handler.HandleAsync(new BaseSoftDeleteCommand<TEntity>(id));

          if (response.Status == CommandResultStatus.SUCCESS)
            return response;

          throw HL.ExecutionError.Create(response.Message, response.ErrorCode, response.Status);
        });
    }
  }
}
