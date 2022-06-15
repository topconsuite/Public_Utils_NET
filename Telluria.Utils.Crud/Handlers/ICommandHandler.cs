using System.Threading.Tasks;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Commands;

namespace Telluria.Utils.Crud.Handlers
{
    public interface ICommandHandler { }

    public interface ICommandHandler<in TCommand, TResultData> : ICommandHandler
      where TCommand : ICommand
    {
        Task<ICommandResult<TResultData>> HandleAsync(TCommand command);
    }

    public interface ICommandHandler<in TCommand> : ICommandHandler
      where TCommand : ICommand
    {
        Task<ICommandResult> HandleAsync(TCommand command);
    }

    public interface IListCommandHandler<in TCommand, TResultData> : ICommandHandler
      where TCommand : IListCommand
    {
        Task<IListCommandResult<TResultData>> HandleAsync(TCommand command);
    }
}
