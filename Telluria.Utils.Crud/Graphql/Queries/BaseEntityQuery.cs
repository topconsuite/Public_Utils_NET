using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Commands.BaseCommands;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Graphql.Types;
using Telluria.Utils.Crud.Handlers;
using HL = Telluria.Utils.Crud.Graphql.Helpers;

namespace Telluria.Utils.Crud.Graphql.Queries
{
  public class BaseEntityQuery<TEntity, TGraphType> : BaseEntityQuery<TEntity, TGraphType, IBaseCrudCommandHandler<TEntity>>
    where TEntity : BaseEntity
    where TGraphType : ObjectGraphType<TEntity>
  {
  }

  public class BaseEntityQuery<TEntity, TGraphType, TCommandHandler> : ObjectGraphType
    where TEntity : BaseEntity
    where TGraphType : ObjectGraphType<TEntity>
    where TCommandHandler : IBaseCrudCommandHandler<TEntity>
  {
    protected void AddBaseQueryGetById()
    {
      var entityName = typeof(TEntity).Name;

      Field<CommandResultType<TEntity, TGraphType>>()
        .Name("GetByID")
        .Argument<NonNullGraphType<GuidGraphType>>("id", $"The id of {entityName}")
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
            new BaseGetCommand<TEntity>(context.GetArgument<Guid>("id"), includes.ToArray())
          );

          if (response.Status == CommandResultStatus.SUCCESS)
            return response;

          throw HL.ExecutionError.Create(response.Message, response.ErrorCode, response.Status);
        });
    }

    protected void AddBaseQueryGetAll()
    {
      Field<ListCommandResultType<TEntity, TGraphType>>()
        .Name("GetAllWithPagination")
        .Argument<IntGraphType>("page", "The page number")
        .Argument<IntGraphType>("perPage", "The number of items per page")
        .Argument<ListGraphType<WhereClausesInputType>>("where", "The where clause")
        .Argument<BooleanGraphType>("includeDeleted", "Include deleted items").DefaultValue(false)
        .ResolveAsync(async context =>
        {
          var includes = new List<string>();
          var result = context?.SubFields?["result"];
          var selections = result?.SelectionSet?.Selections;

          // Get the includes (If has any)
          if (selections != null)
            HL.RecursiveIncludes.AddRecursiveIncludes(selections, includes);

          var contractSellerHandler = context!.RequestServices!.GetRequiredService<TCommandHandler>();
          var page = context.GetArgument<uint>("page");
          var perPage = context.GetArgument<uint>("perPage");

          var where = context.GetArgument<List<WhereClauses>>("where");
          var whereClauses = ParserWhereClauses.Parse<TEntity>(where ?? new List<WhereClauses>());

          var response = context.GetArgument<bool>("includeDeleted")
          ? await contractSellerHandler.HandleAsync(new BaseListAllCommand<TEntity>(page, perPage, whereClauses, includes.ToArray()))
          : await contractSellerHandler.HandleAsync(new BaseListCommand<TEntity>(page, perPage, whereClauses, includes.ToArray()));

          if (response.Status == CommandResultStatus.SUCCESS)
            return response;

          throw HL.ExecutionError.Create(response.Message, response.ErrorCode, response.Status);
        });
    }
  }
}
