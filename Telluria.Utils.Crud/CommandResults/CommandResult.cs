using System.Collections.Generic;
using FluentValidation.Results;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Lists;

namespace Telluria.Utils.Crud.CommandResults
{
  public class CommandResult : ICommandResult
  {
    public ECommandResultStatus Status { get; set; }
    public string Message { get; set; }
    public string ErrorCode { get; set; } = null;
    public IEnumerable<ValidationFailure> Notifications { get; set; } = null;

    public CommandResult(
      ECommandResultStatus status,
      string message,
      string errorCode = null,
      IEnumerable<ValidationFailure> notifications = null)
    {
      Status = status;
      Message = message;
      ErrorCode = errorCode;
      Notifications = notifications;
    }

    public ICommandResult<TResult> ToCommandResult<TResult>()
      where TResult : BaseEntity
    {
      return new CommandResult<TResult>(Status, Message, null!, ErrorCode, Notifications);
    }

    public ICommandResult<IEnumerable<TResult>> ToEnumerableCommandResult<TResult>()
      where TResult : BaseEntity
    {
      return new CommandResult<IEnumerable<TResult>>(Status, Message, null!, ErrorCode, Notifications);
    }

    public IListCommandResult<TResult> ToListCommandResult<TResult>()
      where TResult : BaseEntity
    {
      return new ListCommandResult<TResult>(Status, Message, null!, ErrorCode, Notifications);
    }
  }

  public class CommandResult<TResult> : CommandResult, ICommandResult<TResult>
  {
    public TResult Result { get; set; }

    public CommandResult(
      ECommandResultStatus status,
      string message,
      TResult result,
      string errorCode = null,
      IEnumerable<ValidationFailure> notifications = null)
      : base(status, message, errorCode, notifications)
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
      ECommandResultStatus status,
      string message,
      PagedList<TResult> pagedEntityList,
      string errorCode = null,
      IEnumerable<ValidationFailure> notifications = null)
      : base(status, message, pagedEntityList.Records, errorCode, notifications)
    {
      Page = pagedEntityList.Page;
      PerPage = pagedEntityList.PerPage;
      PageCount = pagedEntityList.PageCount;
      TotalCount = pagedEntityList.TotalCount;
    }
  }
}
