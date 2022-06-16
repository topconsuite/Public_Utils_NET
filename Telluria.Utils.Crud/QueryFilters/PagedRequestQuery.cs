using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.QueryFilters
{
    public class PagedRequestQuery<TEntity> : IPagedRequestQuery, IWhereRequestQuery<TEntity>, IIncludeRequestQuery
      where TEntity : BaseEntity
    {
        public string Where { get; set; }
        public string[] Include { get; set; }
        public uint Page { get; set; } = 1;
        public uint PerPage { get; set; } = 30;
    }
}
