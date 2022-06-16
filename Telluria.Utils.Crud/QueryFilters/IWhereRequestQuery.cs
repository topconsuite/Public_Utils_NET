using System;
using System.Linq.Expressions;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.QueryFilters
{
  public interface IWhereRequestQuery<TEntity>
    where TEntity : BaseEntity
  {
    public string Where { get; set; }
  }

  public static class WhereRequestQueryExtensions
  {
    public static Expression<Func<TEntity, bool>> GetFilter<TEntity>(this IWhereRequestQuery<TEntity> source)
      where TEntity : BaseEntity
    {
      return ExpressionParser.Parse<TEntity>(source.Where);
    }
  }
}
