using Flunt.Notifications;
using System.Collections.Generic;

namespace Telluria.Utils.Crud.CommandResults
{
    public interface ICommandResult
    {
        bool Success { get; }
        string Message { get; }
        IEnumerable<Notification> Notifications { get; }
    }

    public interface ICommandResult<TData> : ICommandResult
    {
        TData Data { get; }
    }

    public interface IListCommandResult<TData> : ICommandResult<IEnumerable<TData>>
    {
        uint Page { get; }
        uint PerPage { get; }
        uint PageCount { get; }
        ulong TotalCount { get; }
    }
}
