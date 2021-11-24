using Flunt.Notifications;
using System.Collections.Generic;
using Telluria.Utils.Crud.Lists;

namespace Telluria.Utils.Crud.CommandResults
{
    public class CommandResult : ICommandResult
    {
        public bool Success { get; }

        public string Message { get; }

        public IEnumerable<Notification> Notifications { get; }

        public CommandResult(bool success, string message, IEnumerable<Notification> notifications)
        {
            Success = success;
            Message = message;
            Notifications = notifications;
        }
    }

    public class CommandResult<TData> : CommandResult, ICommandResult<TData>
    {
        public TData Data { get; }

        public CommandResult(bool success, string message, TData data, IEnumerable<Notification> notifications)
            : base(success, message, notifications)
        {
            Data = data;
        }
    }

    public class ListCommandResult<TData> : CommandResult<IEnumerable<TData>>, IListCommandResult<TData>
    {
        public uint Page { get; }

        public uint PerPage { get; }

        public uint PageCount { get; }

        public ulong TotalCount { get; }

        public ListCommandResult(bool success, string message, PagedList<TData> pagedEntityList, IEnumerable<Notification> notifications)
            : base(success, message, pagedEntityList.Records, notifications)
        {
            Page = pagedEntityList.Page;
            PerPage = pagedEntityList.PerPage;
            PageCount = pagedEntityList.PageCount;
            TotalCount = pagedEntityList.TotalCount;
        }
    }
}
