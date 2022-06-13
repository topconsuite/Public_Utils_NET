using GraphQL.Types;
using Telluria.Utils.Crud.QueryFilters;

namespace Telluria.Utils.Crud.GraphQL.Types
{
  public class WhereClauses
  {
    public WhereClauses() { }

    public string Property { get; set; } = string.Empty;
    public EWhereClausesOperators Operator { get; set; }
    public string Value { get; set; } = string.Empty;
  }

  public class WhereClausesInputType : InputObjectGraphType
  {
    public WhereClausesInputType()
    {
      Name = "WhereClausesInputType";
      Description = "Where clauses input type";

      Field<StringGraphType>("property", "The property to filter");
      Field<EnumerationGraphType<EWhereClausesOperators>>("operator", "The operator to filter");
      Field<StringGraphType>("value", "The value to filter");
    }
  }
}
