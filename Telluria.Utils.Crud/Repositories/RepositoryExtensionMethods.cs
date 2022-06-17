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
    public static IQueryable<TEntity> Tracking<TEntity>(this IQueryable<TEntity> source, bool tracking)
      where TEntity : BaseEntity
    {
      return tracking ? source : source.AsNoTracking();
    }

    public static async Task<PagedList<TEntity>> PagedList<TEntity>(this IQueryable<TEntity> source, uint page, uint perPage, bool tracking = false)
      where TEntity : BaseEntity
    {
      uint maxPageSize = 200;

      if (page < 1) page = 1;
      if (perPage < 1 || perPage > maxPageSize) perPage = maxPageSize;

      var count = (uint)source.Count();

      var pageCount = count / perPage;
      if ((count % perPage) != 0) pageCount++;
      if (page > pageCount && pageCount > 0) page = pageCount;

      var skip = Math.Abs(page - 1) * perPage;

      var records = await source.Tracking(tracking).Skip((int)skip).Take((int)perPage).ToListAsync();

      var result = new PagedList<TEntity>
      {
        Page = page,
        PerPage = perPage,
        PageCount = pageCount,
        TotalCount = count,
        Records = records
      };

      return result;
    }

    public static async Task AddRangeAsync<TEntity>(this DbSet<TEntity> source, params TEntity[] entities)
      where TEntity : BaseEntity
    {
      await Task.Run(() => source.AddRange(entities));
    }

    public static async Task UpdateRangeAsync<TEntity>(this DbSet<TEntity> source, params TEntity[] entities)
      where TEntity : BaseEntity
    {
      await Task.Run(() => source.UpdateRange(entities));
    }

    public static async Task RemoveRangeAsync<TEntity>(this DbSet<TEntity> source, params TEntity[] entities)
      where TEntity : BaseEntity
    {
      await Task.Run(() => source.RemoveRange(entities));
    }
  }
}
