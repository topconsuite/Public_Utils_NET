namespace Telluria.Utils.Crud.Commands
{
  public interface ICommand { }

  public interface IListCommand : ICommand
  {
    uint Page { get; }
    uint PerPage { get; }
  }
}
