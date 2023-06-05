using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.QueryFilters;

public class WhereRequestQuery<TEntity> : IWhereRequestQuery<TEntity>, IIncludeRequestQuery
  where TEntity : BaseEntity
{
  public string[] Include { get; set; }
  public string Where { get; set; }
}
