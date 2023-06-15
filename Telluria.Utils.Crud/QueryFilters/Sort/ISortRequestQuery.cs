using System;
using System.Linq;
using Telluria.Utils.Crud.Constants.Enums;
using Telluria.Utils.Crud.GraphQL.InputTypes;

namespace Telluria.Utils.Crud.QueryFilters;

public interface ISortRequestQuery
{
  string Sort { get; set; }
}

public static class SortRequestQueryExtensions
{
  /// <summary>
  ///   Format string request on array of SortClauses.
  ///   PS: Work without case sensitive.
  /// </summary>
  /// <param name="source"> String (sort clauses). </param>
  /// <returns> Array of SortClauses. </returns>
  /// <example>
  ///   <code>
  ///     var source = "$(createdAt==desc;name==asc)";
  ///     var result = source.GetSorters();
  ///     // result => new SortClauses[] {
  ///     //    new SortClauses("name", ESort.ASC),
  ///     //    new SortClauses("age", ESort.DESC)
  ///     // }
  ///   </code>
  /// </example>
  public static SortClauses[] GetSorters(this ISortRequestQuery source)
  {
    if (string.IsNullOrWhiteSpace(source.Sort)) return Array.Empty<SortClauses>();

    var sorters = source.Sort
      .Replace(" ", "")
      .Replace("$", "")
      .Replace("(", "")
      .Replace(")", "")
      .Split(';')
      .Select(x =>
      {
        var sort = x.Split("==");
        return new SortClauses(sort[0], sort[1].ToLower() == "desc" ? ESort.DESC : ESort.ASC);
      })
      .ToArray();

    return sorters;
  }
}
