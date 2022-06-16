using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.QueryFilters
{
  public class WhereRequestQuery<TEntity> : IWhereRequestQuery<TEntity>, IIncludeRequestQuery
    where TEntity : BaseEntity
  {
    public string Where { get; set; }
    public string[] Include { get; set; }
  }
}
