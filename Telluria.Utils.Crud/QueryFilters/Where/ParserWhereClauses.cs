using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Telluria.Utils.Crud.GraphQL.Types;

namespace Telluria.Utils.Crud.QueryFilters;

public static class ParserWhereClauses
{
  public static Expression<Func<TEntity, bool>> Parse<TEntity>(
    IEnumerable<WhereClauses> whereClauses,
    bool caseSensitive = true)
    where TEntity : class
  {
    var whereQuery = new StringBuilder("$(");

    foreach (var whereClause in whereClauses)
    {
      whereQuery.Append($"{whereClause.Property}{whereClause.Operator.GetOperator()}{whereClause.Value}");

      // If it is not the last clause, add a separator
      if (whereClauses.Last() != whereClause)
        whereQuery.Append(';');
    }

    whereQuery.Append(')');

    return ExpressionParser.Parse<TEntity>(whereQuery.ToString(), caseSensitive);
  }
}
