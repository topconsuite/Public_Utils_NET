namespace Telluria.Utils.Crud.Constants.Enums
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
      switch (@operator)
      {
        case EWhereClausesOperators.Equals: return "==";
        case EWhereClausesOperators.GreaterThanOrEqual: return ">=";
        case EWhereClausesOperators.LessThanOrEqual: return "<=";
        case EWhereClausesOperators.GreaterThan: return ">>";
        case EWhereClausesOperators.LessThan: return "<<";
        case EWhereClausesOperators.Contains: return "%=";
        case EWhereClausesOperators.In: return "%>";
        default: return string.Empty;
      }
    }
  }
}
