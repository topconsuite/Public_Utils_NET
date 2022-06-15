using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Lists;

namespace Telluria.Utils.Crud.Repositories
{
  public static class RepositoryExtensionMethods
  {
    public static IQueryable<T> Tracking<T>(this IQueryable<T> source, bool tracking)
      where T : BaseEntity
    {
      if (tracking)
        return source;
      else
        return source.AsNoTracking();
    }

    public static async Task<PagedList<T>> PagedList<T>(this IQueryable<T> source, uint page, uint perPage, bool tracking = false)
      where T : BaseEntity
    {
      uint _maxPageSize = 200;

      if (page < 1) page = 1;
      if (perPage < 1 || perPage > _maxPageSize) perPage = _maxPageSize;

      var count = (uint)source.Count();

      var pageCount = count / perPage;
      if ((count % perPage) != 0) pageCount++;
      if (page > pageCount && pageCount > 0) page = pageCount;

      var skip = Math.Abs(page - 1) * perPage;

      var records = await source.Tracking(tracking).Skip((int)skip).Take((int)perPage).ToListAsync();

      var result = new PagedList<T>
      {
        Page = page,
        PerPage = perPage,
        PageCount = pageCount,
        TotalCount = count,
        Records = records
      };

      return result;
    }
  }
}
