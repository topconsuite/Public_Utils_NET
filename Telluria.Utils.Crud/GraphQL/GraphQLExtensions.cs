using System.Collections.Generic;
using GraphQL;
using Telluria.Utils.Crud.QueryFilters;

namespace Telluria.Utils.Crud.GraphQL
{
  public static class GraphQLExtensions
  {
    public static string[] GetIncludes<TSourceType>(this IResolveFieldContext<TSourceType> source)
    {
      var includes = new List<string>();
      var result = source?.SubFields?["result"];
      var selections = result?.SelectionSet?.Selections;

      // Get the includes (If has any)
      if (selections != null)
        RecursiveIncludes.AddRecursiveIncludes(selections, includes);

      return includes.ToArray();
    }
  }
}
