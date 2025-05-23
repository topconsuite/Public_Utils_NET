using System.Collections.Generic;
using GraphQLParser.AST;
using Telluria.Utils.Crud.Helpers;

namespace Telluria.Utils.Crud.QueryFilters
{
  public static class RecursiveIncludes
  {
    public static string AddRecursiveIncludes(List<ASTNode> selectionList, IList<string> includes, string aux = "")
    {
      var fullName = aux;

      foreach (var selection in selectionList)
      {
        var selectionSet = selection?.GetType()?.GetProperty("SelectionSet")?.GetValue(selection);
        var selectionSetSelections = (List<ASTNode>)(selectionSet?.GetType()?.GetProperty("Selections")?.GetValue(selectionSet)
          ?? new List<ASTNode>());

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
