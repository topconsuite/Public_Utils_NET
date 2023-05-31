using GraphQL.Types;
using Telluria.Utils.Crud.Constants.Enums;

namespace elluria.Utils.Crud.GraphQL.InputTypes;

public class SortClauses
{
  public string Field { get; set; } = string.Empty;
  public ESort SortDirection { get; set; }
}

public class SortInputType : InputObjectGraphType
{
  public SortInputType()
  {
    Name = "SortInputType";
    Description = "Sort input type";

    Field<StringGraphType>("field", "The property to filter");
    Field<EnumerationGraphType<ESort>>("sortDirection", "The value to filter");
  }
}
