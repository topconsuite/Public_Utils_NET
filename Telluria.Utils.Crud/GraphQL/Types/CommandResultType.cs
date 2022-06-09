using GraphQL.Types;
using Telluria.Utils.Crud.CommandResults;

namespace Telluria.Utils.Crud.GraphQL.Types
{
  public class CommandResultType : ObjectGraphType<ICommandResult>
  {
    public CommandResultType()
    {
      Name = $"CommandResult_Void";
      Description = "The result of a command";

      Field(x => x.Status, nullable: false, type: typeof(EnumerationGraphType<CommandResultStatus>)).Description("The status of the command");
      Field(x => x.Message, nullable: false).Description("The message of the command");
      Field(x => x.ErrorCode, nullable: true).Description("The error code of the command");
      Field(x => x.Notifications, nullable: true, type: typeof(ListGraphType<ValidationFailureType>)).Description("The notifications of the command");
    }
  }

  public class CommandResultType<TEntity, TGraphType> : ObjectGraphType<ICommandResult<TEntity>>
  where TGraphType : ObjectGraphType<TEntity>
  {
    public CommandResultType()
    {
      Name = $"CommandResult_{typeof(TEntity).Name}";
      Description = "The result of a command";

      Field(x => x.Status, nullable: false, type: typeof(EnumerationGraphType<CommandResultStatus>)).Description("The status of the command");
      Field(x => x.Message, nullable: false).Description("The message of the command");
      Field(x => x.Result, nullable: true, type: typeof(TGraphType)).Description("The result of the command");
      Field(x => x.ErrorCode, nullable: true).Description("The error code of the command");
      Field(x => x.Notifications, nullable: true, type: typeof(ListGraphType<ValidationFailureType>)).Description("The notifications of the command");
    }
  }
}
