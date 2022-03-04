using System.Collections.Generic;
using Telluria.Utils.Crud.Lists;

namespace Telluria.Utils.Crud.CommandResults
{
  public class CommandResult : ICommandResult
  {
    public CommandResultStatus Status { get; set; }
    public string Message { get; set; }
    public string ErrorCode { get; set; } = null;
    public IEnumerable<FluentValidation.Results.ValidationFailure> Notifications { get; set; } = null;
    public System.Exception Exception { get; set; } = null;

    public CommandResult(
      CommandResultStatus status,
      string message,
      string errorCode = null,
      IEnumerable<FluentValidation.Results.ValidationFailure> notifications = null,
      System.Exception exception = null)
    {
      Status = status;
      Message = message;
      ErrorCode = errorCode;
      Notifications = notifications;
      Exception = exception;
    }
  }

  public class CommandResult<TResult> : CommandResult, ICommandResult<TResult>
  {
    public TResult Result { get; set; }

    public CommandResult(
      CommandResultStatus status,
      string message,
      TResult result,
      string errorCode = null,
      IEnumerable<FluentValidation.Results.ValidationFailure> notifications = null,
      System.Exception exception = null)
    : base(status, message, errorCode, notifications, exception)
    {
      Result = result;
    }
  }

  public class ListCommandResult<TResult> : CommandResult<IEnumerable<TResult>>, IListCommandResult<TResult>
  {
    public uint Page { get; }
    public uint PerPage { get; }
    public uint PageCount { get; }
    public ulong TotalCount { get; }

    public ListCommandResult(
      CommandResultStatus status,
      string message,
      PagedList<TResult> pagedEntityList,
      string errorCode = null,
      IEnumerable<FluentValidation.Results.ValidationFailure> notifications = null,
      System.Exception exception = null)
    : base(status, message, pagedEntityList.Records, errorCode, notifications, exception)
    {
      Page = pagedEntityList.Page;
      PerPage = pagedEntityList.PerPage;
      PageCount = pagedEntityList.PageCount;
      TotalCount = pagedEntityList.TotalCount;
    }
  }
}
