using System.Linq;
using System.Linq.Expressions;

namespace Telluria.Utils.Crud.Helpers;

public static class QueryableExtension
{
  /// <summary>
  ///   OrderBy extension method for IQueryable.
  /// </summary>
  /// <param name="source"> IQueryable. </param>
  /// <param name="propertyName"> Property name. </param>
  /// <param name="descending"> Boolean indicating descending. </param>
  /// <typeparam name="T"> Type of IQueryable. </typeparam>
  /// <returns> IOrderedQueryable (ORDER BY). </returns>
  public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool descending = false)
  {
    var param = Expression.Parameter(typeof(T), "p");
    var prop = Expression.Property(param, propertyName);
    var lambda = Expression.Lambda(prop, param);

    var method = descending ? "OrderByDescending" : "OrderBy";
    var orderByCall = Expression.Call(
      typeof(Queryable),
      method,
      new[] { typeof(T), prop.Type },
      source.Expression,
      Expression.Quote(lambda));

    return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(orderByCall);
  }

  /// <summary>
  ///   ThenBy extension method for IOrderedQueryable.
  /// </summary>
  /// <param name="source"> IOrderedQueryable. </param>
  /// <param name="propertyName"> Property name. </param>
  /// <param name="descending"> Boolean indicating descending. </param>
  /// <typeparam name="T"> Type of IOrderedQueryable. </typeparam>
  /// <returns> IOrderedQueryable (Then BY). </returns>
  public static IOrderedQueryable<T> ThenBy<T>(
    this IOrderedQueryable<T> source, string propertyName, bool descending = false)
  {
    var param = Expression.Parameter(typeof(T), "p");
    var prop = Expression.Property(param, propertyName);
    var lambda = Expression.Lambda(prop, param);

    var method = descending ? "ThenByDescending" : "ThenBy";
    var orderByCall = Expression.Call(
      typeof(Queryable),
      method,
      new[] { typeof(T), prop.Type },
      source.Expression,
      Expression.Quote(lambda));

    return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(orderByCall);
  }
}
