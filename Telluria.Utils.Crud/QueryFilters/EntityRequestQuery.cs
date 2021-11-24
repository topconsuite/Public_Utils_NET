using System;
using System.Linq;
using System.Linq.Expressions;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Helpers;

namespace Telluria.Utils.Crud.QueryFilters
{
    public class EntityRequestQuery<TEntity> where TEntity : BaseEntity
    {
        public string Where { get; set; }
        public string[] Include { get; set; }
        public uint Page { get; set; } = 1;
        public uint PerPage { get; set; } = 30;

        public Expression<Func<TEntity, bool>> GetFilter()
        {
            return ExpressionParser.Parse<TEntity>(Where);
        }

        public string[] GetIncludes()
        {
            return Include?
                .Select(i => string.Join('.', i.Split('.').Select(t => t.FirstCharToUpper())))
                .ToArray() ?? new string[] { };
        }
    }
}
