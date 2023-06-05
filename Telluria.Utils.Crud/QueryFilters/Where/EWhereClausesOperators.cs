namespace Telluria.Utils.Crud.QueryFilters
{
  public enum EWhereClausesOperators
  {
    Equals, // ==
    GreaterThanOrEqual, // >=
    LessThanOrEqual, // <=
    GreaterThan, // >>
    LessThan, // <<
    Contains, // %=
    In, // %> (For this option, values must be separated by "|")
  }

  public static class EWhereClausesOperatorsExtensions
  {
    public static string GetOperator(this EWhereClausesOperators @operator)
    {
      return @operator switch
      {
        EWhereClausesOperators.Equals => "==",
        EWhereClausesOperators.GreaterThanOrEqual => ">=",
        EWhereClausesOperators.LessThanOrEqual => "<=",
        EWhereClausesOperators.GreaterThan => ">>",
        EWhereClausesOperators.LessThan => "<<",
        EWhereClausesOperators.Contains => "%=",
        EWhereClausesOperators.In => "%>",
        _ => string.Empty,
      };
    }
  }
}
