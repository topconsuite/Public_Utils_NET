using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.GraphQL.InputTypes;

namespace Telluria.Utils.Crud.QueryFilters;

public class PagedRequestQuery<TEntity> : IPagedRequestQuery, IWhereRequestQuery<TEntity>, IIncludeRequestQuery
  where TEntity : BaseEntity
{
  public SortClauses Sort { get; set; }
  public string[] Include { get; set; }
  public uint Page { get; set; } = 1;
  public uint PerPage { get; set; } = 30;
  public string Where { get; set; }
}
