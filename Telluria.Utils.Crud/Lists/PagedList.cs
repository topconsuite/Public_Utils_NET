using System.Collections.Generic;

namespace Telluria.Utils.Crud.Lists
{
    public class PagedList<TEntity>
    {
        public uint Page { get; set; }
        public uint PerPage { get; set; }
        public uint PageCount { get; set; }
        public ulong TotalCount { get; set; }
        public IEnumerable<TEntity> Records { get; set; }
    }
}
