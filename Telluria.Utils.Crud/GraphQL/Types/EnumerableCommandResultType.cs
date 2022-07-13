using System.Collections.Generic;
using GraphQL.Types;
using Telluria.Utils.Crud.CommandResults;

namespace Telluria.Utils.Crud.GraphQL.Types;

public class EnumerableCommandResultType<TEntity, TGraphType> : ObjectGraphType<ICommandResult<IEnumerable<TEntity>>>
  where TGraphType : ObjectGraphType<TEntity>
{
  public EnumerableCommandResultType()
  {
    Name = $"EnumerableCommandResult_{typeof(TEntity).Name}";
    Description = "The result of a command (Enumerable)";

    Field(x => x.Status, false, typeof(EnumerationGraphType<ECommandResultStatus>))
      .Description("The status of the command");
    Field(x => x.Message, false).Description("The message of the command");
    Field(x => x.Result, true, typeof(ListGraphType<TGraphType>))
      .Description("The result of the command");
    Field(x => x.ErrorCode, true).Description("The error code of the command");
    Field(x => x.Notifications, true, typeof(ListGraphType<ValidationFailureType>))
      .Description("The notifications of the command");
  }
}
