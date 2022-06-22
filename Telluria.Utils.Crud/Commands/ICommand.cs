using System.Threading;

namespace Telluria.Utils.Crud.Commands
{
  public interface ICommand
  {
    CancellationToken CancellationToken { get; }
  }

  public interface IListCommand : ICommand
  {
    uint Page { get; }
    uint PerPage { get; }
  }
}
