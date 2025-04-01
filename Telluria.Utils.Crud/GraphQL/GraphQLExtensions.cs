using GraphQL;
using GraphQLParser.AST;
using System.Collections.Generic;
using System.Linq;

namespace Telluria.Utils.Crud.GraphQL
{
  public static class GraphQLExtensions
  {
    public static string[] GetIncludes<TSourceType>(this IResolveFieldContext<TSourceType> context)
    {
      var includes = new List<string>();

      // In v8, use FieldAst to get the current field’s AST.
      // We assume the client requested a subfield named "result".
      var resultField = context.FieldAst?.SelectionSet?.Selections
          .OfType<GraphQLField>()
          .FirstOrDefault(f => f.Name.StringValue == "result");

      if (resultField == null || resultField.SelectionSet == null)
        return includes.ToArray();

      // Recursively process the selections under "result".
      RecursiveIncludes.AddRecursiveIncludes(resultField.SelectionSet.Selections, includes);

      return includes.ToArray();
    }
  }

  public static class RecursiveIncludes
  {
    public static string AddRecursiveIncludes(IEnumerable<ASTNode> selections, IList<string> includes, string aux = "")
    {
      string fullName = aux;

      foreach (var selection in selections)
      {
        if (selection is GraphQLField field && field.SelectionSet != null && field.SelectionSet.Selections.Any())
        {
          var name = field.Name.StringValue;
          if (!string.IsNullOrEmpty(name))
          {
            fullName = !string.IsNullOrEmpty(aux)
                ? aux + "." + name.ToPascalCase()
                : name.ToPascalCase();

            var childIncludes = AddRecursiveIncludes(field.SelectionSet.Selections, includes, fullName);
            if (!string.IsNullOrEmpty(childIncludes))
              includes.Add(fullName.ToPascalCase());
          }
        }
      }

      return fullName;
    }
  }
}
