using System.Collections.Generic;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Constants.Errors.HTTP;
using GQL = GraphQL;

namespace Telluria.Utils.Crud.GraphQL.Helpers
{
  public static class ExecutionError
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
    public static GQL.ExecutionError Create(
      string message = null!,
      string errorCode = null!,
      CommandResultStatus status = CommandResultStatus.ERROR)
    {
      return new GQL.ExecutionError(message ?? _message, new Dictionary<string, string>
      {
        { "errorCode", errorCode ?? _errorCode },
        { "status", status.ToString() }
      });
    }
  }
}
