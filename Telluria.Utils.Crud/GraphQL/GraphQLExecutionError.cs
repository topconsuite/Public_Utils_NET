using System.Collections.Generic;
using GraphQL;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Errors;

namespace Telluria.Utils.Crud.GraphQL
{
  public static class GraphQLExecutionError
  {
    private static readonly string _message = EServerErrorExtensions.GetErrorMessage(EServerError.INTERNAL_SERVER_ERROR);
    private static readonly string _errorCode = EServerErrorExtensions.GetErrorCode(EServerError.INTERNAL_SERVER_ERROR);

    /// <summary>
    /// Creates a GraphQL execution error.
    /// </summary>
    /// <param name="message"> The error message. </param>
    /// <param name="errorCode"> The error code. </param>
    /// <param name="status"> The pre defined status code. </param>
    /// <returns> The GraphQL execution error. </returns>
    public static ExecutionError Create(
      string message = null!,
      string errorCode = null!,
      ECommandResultStatus status = ECommandResultStatus.ERROR)
    {
      var executionError = new ExecutionError(message ?? _message);

      executionError.AddExtension("data", new Dictionary<string, string>
      {
        { "errorCode", errorCode ?? _errorCode },
        { "status", status.ToString() }
      });

      return executionError;
    }
  }
}
