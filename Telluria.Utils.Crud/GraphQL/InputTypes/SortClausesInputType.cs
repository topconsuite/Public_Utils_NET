using GraphQL.Types;
using Telluria.Utils.Crud.Constants.Enums;

namespace Telluria.Utils.Crud.GraphQL.InputTypes;

public class SortClauses
{
  public SortClauses()
  {
  }

  public SortClauses(string field, ESort descSortDirection)
  {
    Field = field;
    SortDirection = descSortDirection;
  }

  public string Field { get; set; }
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
