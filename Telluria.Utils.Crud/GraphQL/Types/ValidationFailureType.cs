using FluentValidation.Results;
using GraphQL.Types;

namespace Telluria.Utils.Crud.GraphQL.Types
{
  public class ValidationFailureType : ObjectGraphType<ValidationFailure>
  {
    public ValidationFailureType()
    {
      Name = "Notification";
      Description = "Notification";

      Field(x => x.ErrorCode, type: typeof(NonNullGraphType<StringGraphType>)).Description("The error code");
      Field(x => x.PropertyName, type: typeof(NonNullGraphType<StringGraphType>)).Description("The property name");
      Field(x => x.ErrorMessage, type: typeof(NonNullGraphType<StringGraphType>)).Description("The error message");
    }
  }
}
