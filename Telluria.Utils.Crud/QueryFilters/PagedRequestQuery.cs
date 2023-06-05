using Telluria.Utils.Crud.Entities;
namespace Telluria.Utils.Crud.QueryFilters;

public class PagedRequestQuery<TEntity> : IPagedRequestQuery, IWhereRequestQuery<TEntity>, IIncludeRequestQuery,
  ISortRequestQuery
  where TEntity : BaseEntity
{
  public string Sort { get; set; }
  public bool CaseSensitive { get; set; } = true;
  public string[] Include { get; set; }
  public uint Page { get; set; } = 1;
  public uint PerPage { get; set; } = 30;
  public string Where { get; set; }
}
