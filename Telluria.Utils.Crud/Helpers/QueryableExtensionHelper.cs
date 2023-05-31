using System;
using System.Linq;
using System.Linq.Expressions;

namespace Telluria.Utils.Crud.Helpers;

public static class QueryableExtension
{
  public static IQueryable<T> OrderByField<T>(this IQueryable<T> q, string sortColumn, bool asc)
  {
    var param = Expression.Parameter(typeof(T), "p");
    var prop = Expression.Property(param, sortColumn);
    var exp = Expression.Lambda(prop, param);
    var method = asc ? "OrderBy" : "OrderByDescending";

    Type[] types = { q.ElementType, exp.Body.Type };

    var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);

    return q.Provider.CreateQuery<T>(mce);
  }
}
