using System.Collections.Generic;
using Flunt.Notifications;
using Telluria.Utils.Crud.Lists;

namespace Telluria.Utils.Crud.CommandResults
{
  public class CommandResult : ICommandResult
  {
    public CommandResultStatus Status { get; }
    public string Message { get; }
    public IEnumerable<Notification> Notifications { get; }

    public CommandResult(CommandResultStatus status, string message, IEnumerable<Notification> notifications)
    {
      Status = status;
      Message = message;
      Notifications = notifications;
    }
  }

  public class CommandResult<TResult> : CommandResult, ICommandResult<TResult>
  {
    public TResult Result { get; }

    public CommandResult(CommandResultStatus status, string message, TResult result, IEnumerable<Notification> notifications)
        : base(status, message, notifications)
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

    public ListCommandResult(CommandResultStatus status, string message, PagedList<TResult> pagedEntityList, IEnumerable<Notification> notifications)
        : base(status, message, pagedEntityList.Records, notifications)
    {
      Page = pagedEntityList.Page;
      PerPage = pagedEntityList.PerPage;
      PageCount = pagedEntityList.PageCount;
      TotalCount = pagedEntityList.TotalCount;
    }
  }
}
