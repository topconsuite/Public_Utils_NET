using System;
using System.Linq;
using Telluria.Utils.Crud.Helpers;

namespace Telluria.Utils.Crud.QueryFilters
{
  public interface IIncludeRequestQuery
  {
    public string[] Include { get; set; }
  }

  public static class IncludeRequestQueryExtensions
  {
    public static string[] GetIncludes(this IIncludeRequestQuery source)
    {
      return source.Include?
        .Select(i => string.Join('.', i.Split('.').Select(t => t.FirstCharToUpper())))
        .ToArray() ?? Array.Empty<string>();
    }
  }
}
