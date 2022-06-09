using System.Collections;
using GraphQL;
using GraphQL.Language.AST;

namespace Telluria.Utils.Crud.Graphql.Helpers
{
  public static class RecursiveIncludes
  {
    public static string AddRecursiveIncludes(IList<ISelection> selectionList, IList<string> includes, string aux = "")
    {
      var fullName = aux;

      foreach (var selection in selectionList)
      {
        var selectionSet = selection?.GetType()?.GetProperty("SelectionSet")?.GetValue(selection);
        var selectionSetSelections = (IList<ISelection>)(selectionSet?.GetType()?.GetProperty("Selections")?.GetValue(selectionSet)
          ?? new List<ISelection>());

        if (selectionSetSelections?.Count > 0)
        {
          var name = (string)(selection?.GetType()?.GetProperty("Name")?.GetValue(selection) ?? "");

          if (!string.IsNullOrEmpty(name))
          {
            fullName = !string.IsNullOrEmpty(aux) ? aux + "." + name.ToPascalCase() : name.ToPascalCase();
            var childIncludes = AddRecursiveIncludes(selectionSetSelections, includes, fullName);

            if (!string.IsNullOrEmpty(childIncludes))
              includes.Add(fullName.ToPascalCase());
          }
        }
      }

      return fullName;
    }
  }
}
