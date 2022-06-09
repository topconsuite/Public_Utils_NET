using GraphQL.Types;
using Telluria.Utils.Crud.CommandResults;

namespace Telluria.Utils.Crud.GraphQL.Types
{
  public class ListCommandResultType<TEntity, TGraphType> : ObjectGraphType<ListCommandResult<TEntity>>
  where TGraphType : ObjectGraphType<TEntity>
  {
    public ListCommandResultType()
    {
      Name = $"ListCommandResult_{typeof(TEntity).Name}";
      Description = "A list of items (GraphQL Type)";

      Field(x => x.Status, nullable: false, type: typeof(EnumerationGraphType<CommandResultStatus>)).Description("The status of the command");
      Field(x => x.Message, nullable: false).Description("The message of the command");
      Field(x => x.Result, nullable: true, type: typeof(ListGraphType<TGraphType>)).Description("The result of the command");
      Field(x => x.ErrorCode, nullable: true).Description("The error code of the command");
      Field(x => x.Notifications, nullable: true, type: typeof(ListGraphType<ValidationFailureType>)).Description("The notifications of the command");

      Field(x => x.Page, nullable: false).Description("The page of the command");
      Field(x => x.PerPage, nullable: false).Description("The per page of the command");
      Field(x => x.PageCount, nullable: false).Description("The page count of the command");
      Field(x => x.TotalCount, nullable: false).Description("The total count of the command");
    }
  }
}
