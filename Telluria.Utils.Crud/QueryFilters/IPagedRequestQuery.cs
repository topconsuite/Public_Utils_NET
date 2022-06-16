namespace Telluria.Utils.Crud.QueryFilters
{
  public interface IPagedRequestQuery
  {
    uint Page { get; set; }
    uint PerPage { get; set; }
  }
}
