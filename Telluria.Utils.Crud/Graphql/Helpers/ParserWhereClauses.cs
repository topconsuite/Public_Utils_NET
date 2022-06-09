using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Telluria.Utils.Crud.Constants.Enums;
using Telluria.Utils.Crud.GraphQL.Types;
using Telluria.Utils.Crud.QueryFilters;

namespace Telluria.Utils.Crud.GraphQL.Helpers
{
  public static class ParserWhereClauses
  {
    public static Expression<Func<TEntity, bool>> Parse<TEntity>(IEnumerable<WhereClauses> whereClauses)
    where TEntity : class
    {
      var whereString = "$(";

      whereClauses.ToList().ForEach(whereClause =>
      {
        whereString += $"{whereClause.Property}{EWhereClausesOperatorsExtensions.GetOperator(whereClause.Operator)}{whereClause.Value}";

        // If it is not the last clause, add a separator
        if (whereClauses.Last() != whereClause)
          whereString += ";";
      });

      whereString += ")";

      return ExpressionParser.Parse<TEntity>(whereString);
    }
  }
}
