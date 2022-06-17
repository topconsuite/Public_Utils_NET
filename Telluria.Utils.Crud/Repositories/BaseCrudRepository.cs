using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Lists;

namespace Telluria.Utils.Crud.Repositories
{
  public abstract class BaseCrudRepository<TEntity> : IBaseCrudRepository<TEntity>
    where TEntity : BaseEntity
  {
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "<Pendente>")]
    protected readonly DbContext _context;

    protected BaseCrudRepository(DbContext context)
    {
      _context = context;
    }

    protected DbSet<TSpecificEntity> DbSet<TSpecificEntity>()
      where TSpecificEntity : BaseEntity
    {
      return _context.Set<TSpecificEntity>();
    }

    private async Task ValidateExistence<TSpecificEntity>(params TSpecificEntity[] entities)
      where TSpecificEntity : BaseEntity
    {
      if (entities.Any(entity => entity.Id == Guid.Empty))
        throw new Exception($"Id '{Guid.Empty}' not found");

      var entitiesIds = entities.Select(x => x.Id);
      var oldEntities = await ListAsync<TSpecificEntity>(false, x => entitiesIds.Contains(x.Id));

      foreach (var entity in entities)
      {
        var oldEntity = oldEntities.FirstOrDefault(x => x.Id == entity.Id);

        if (oldEntity == null)
          throw new Exception($"Id '{entity.Id}' not found");

        entity.CreatedAt = oldEntity.CreatedAt;
        entity.UpdatedAt = oldEntity.UpdatedAt;
        entity.DeletedAt = oldEntity.DeletedAt;
        entity.Deleted = oldEntity.Deleted;
      }
    }

    public virtual async Task AddAsync(params TEntity[] entities)
    {
      await AddAsync<TEntity>(entities);
    }

    public virtual async Task AddAsync<TSpecificEntity>(params TSpecificEntity[] entities)
      where TSpecificEntity : BaseEntity
    {
      var now = DateTime.Now.ToUniversalTime();

      foreach (var entity in entities)
      {
        entity.CreatedAt = now;
        entity.UpdatedAt = null;
        entity.DeletedAt = null;
        entity.Deleted = false;
      }

      await DbSet<TSpecificEntity>().AddRangeAsync(entities);
    }

    public virtual async Task<TEntity> GetAsync(
      Guid id,
      bool tracking = false,
      params string[] includeProperties)
    {
      return await GetAsync<TEntity>(id, tracking, includeProperties);
    }

    public virtual async Task<TSpecificEntity> GetAsync<TSpecificEntity>(
      Guid id,
      bool tracking = false,
      params string[] includeProperties)
      where TSpecificEntity : BaseEntity
    {
      var set = DbSet<TSpecificEntity>().AsQueryable();

      foreach (var includePropertie in includeProperties)
        set = set.Include(includePropertie);

      set = set.Where(t => t.Id == id);

      return await set.Tracking(tracking).FirstOrDefaultAsync();
    }

    public virtual async Task UpdateAsync(params TEntity[] entities)
    {
      await UpdateAsync<TEntity>(entities);
    }

    public virtual async Task UpdateAsync<TSpecificEntity>(params TSpecificEntity[] entities)
      where TSpecificEntity : BaseEntity
    {
      await ValidateExistence(entities);
      var now = DateTime.Now.ToUniversalTime();

      foreach (var entity in entities)
      {
        entity.UpdatedAt = now;
      }

      await DbSet<TSpecificEntity>().UpdateRangeAsync(entities);
    }

    public virtual async Task UpsertAsync(
      TEntity entity,
      Expression<Func<TEntity, object>> match,
      Expression<Func<TEntity, TEntity, TEntity>> updater)
    {
      await UpsertAsync<TEntity>(entity, match, updater);
    }

    public virtual async Task UpsertAsync<TSpecificEntity>(
      TEntity entity,
      Expression<Func<TEntity, object>> match,
      Expression<Func<TEntity, TEntity, TEntity>> updater)
      where TSpecificEntity : BaseEntity
    {
      var now = DateTime.Now.ToUniversalTime();

      entity.CreatedAt = now;

      await DbSet<TEntity>().Upsert(entity).On(match).WhenMatched(updater).RunAsync();
    }

    public virtual async Task SoftDeleteAsync(params TEntity[] entities)
    {
      await SoftDeleteAsync<TEntity>(entities);
    }

    public virtual async Task SoftDeleteAsync<TSpecificEntity>(params TSpecificEntity[] entities)
      where TSpecificEntity : BaseEntity
    {
      await ValidateExistence(entities);
      var now = DateTime.Now.ToUniversalTime();

      foreach (var entity in entities)
      {
        entity.DeletedAt = now;
        entity.Deleted = true;
      }

      // Verify if have foreign key conflicts to continue or not with soft delete
      using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
      {
        await DbSet<TSpecificEntity>().RemoveRangeAsync(entities);
        await Commit();
      }

      await DbSet<TSpecificEntity>().UpdateRangeAsync(entities);
    }

    public virtual async Task RemoveAsync(params TEntity[] entities)
    {
      await RemoveAsync<TEntity>(entities);
    }

    public virtual async Task RemoveAsync<TSpecificEntity>(params TSpecificEntity[] entities)
      where TSpecificEntity : BaseEntity
    {
      await DbSet<TSpecificEntity>().RemoveRangeAsync(entities);
    }

    public virtual async Task<TEntity> FindAsync(
      bool tracking = false,
      Expression<Func<TEntity, bool>> filter = null,
      params string[] includeProperties)
    {
      return await FindAsync<TEntity>(tracking, filter, includeProperties);
    }

    public virtual async Task<TSpecificEntity> FindAsync<TSpecificEntity>(
      bool tracking = false,
      Expression<Func<TSpecificEntity, bool>> filter = null,
      params string[] includeProperties)
      where TSpecificEntity : BaseEntity
    {
      var set = DbSet<TSpecificEntity>().AsQueryable();

      foreach (var includePropertie in includeProperties)
        set = set.Include(includePropertie);

      if (filter != null)
        set = set.Where(filter);

      return await set.Tracking(tracking).FirstOrDefaultAsync();
    }

    public virtual async Task<PagedList<TEntity>> ListAsync(
      uint page,
      uint perPage,
      bool tracking = false,
      Expression<Func<TEntity, bool>> filter = null,
      params string[] includeProperties)
    {
      return await ListAsync<TEntity>(page, perPage, tracking, filter, includeProperties);
    }

    public virtual async Task<PagedList<TSpecificEntity>> ListAsync<TSpecificEntity>(
      uint page,
      uint perPage,
      bool tracking = false,
      Expression<Func<TSpecificEntity, bool>> filter = null,
      params string[] includeProperties)
      where TSpecificEntity : BaseEntity
    {
      var set = DbSet<TSpecificEntity>().AsQueryable();

      foreach (var includePropertie in includeProperties)
        set = set.Include(includePropertie);

      if (filter != null)
        set = set.Where(filter);

      return await set.PagedList(page, perPage, tracking);
    }

    public virtual async Task<IEnumerable<TEntity>> ListAsync(
      bool tracking = false,
      Expression<Func<TEntity, bool>> filter = null,
      params string[] includeProperties)
    {
      return await ListAsync<TEntity>(tracking, filter, includeProperties);
    }

    public virtual async Task<IEnumerable<TSpecificEntity>> ListAsync<TSpecificEntity>(
      bool tracking = false,
      Expression<Func<TSpecificEntity, bool>> filter = null,
      params string[] includeProperties)
      where TSpecificEntity : BaseEntity
    {
      var set = DbSet<TSpecificEntity>().AsQueryable();

      foreach (var includePropertie in includeProperties)
        set = set.Include(includePropertie);

      if (filter != null)
        set = set.Where(filter);

      return await set.Tracking(tracking).ToListAsync();
    }

    public virtual async Task<PagedList<TEntity>> ListAllAsync(
      uint page,
      uint perPage,
      bool tracking = false,
      Expression<Func<TEntity, bool>> filter = null,
      params string[] includeProperties)
    {
      return await ListAllAsync<TEntity>(page, perPage, tracking, filter, includeProperties);
    }

    public virtual async Task<PagedList<TSpecificEntity>> ListAllAsync<TSpecificEntity>(
      uint page,
      uint perPage,
      bool tracking = false,
      Expression<Func<TSpecificEntity, bool>> filter = null,
      params string[] includeProperties)
        where TSpecificEntity : BaseEntity
    {
      var set = DbSet<TSpecificEntity>().AsQueryable();

      foreach (var includePropertie in includeProperties)
        set = set.Include(includePropertie);

      if (filter != null)
        set = set.Where(filter);

      return await set.IgnoreQueryFilters().PagedList(page, perPage, tracking);
    }

    public virtual async Task<IEnumerable<TEntity>> ListAllAsync(
      bool tracking = false,
      Expression<Func<TEntity, bool>> filter = null,
      params string[] includeProperties)
    {
      return await ListAllAsync<TEntity>(tracking, filter, includeProperties);
    }

    public virtual async Task<IEnumerable<TSpecificEntity>> ListAllAsync<TSpecificEntity>(
      bool tracking = false,
      Expression<Func<TSpecificEntity, bool>> filter = null,
      params string[] includeProperties)
      where TSpecificEntity : BaseEntity
    {
      var set = DbSet<TSpecificEntity>().AsQueryable();

      foreach (var includePropertie in includeProperties)
        set = set.Include(includePropertie);

      if (filter != null)
        set = set.Where(filter);

      return await set.IgnoreQueryFilters().Tracking(tracking).ToListAsync();
    }

    public async Task Commit()
    {
      Exception shouldThrow = null;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (Exception e)
      {
        await _context.DisposeAsync();
        shouldThrow = e;
      }

      if (shouldThrow != null) throw shouldThrow;
    }
  }

  public static class TestExtensions
  {
    public static string ToSql<TEntity>(this IQueryable<TEntity> query)
      where TEntity : class
    {
      var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
      var relationalCommandCache = enumerator.Private("_relationalCommandCache");
      var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
      var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

      var sqlGenerator = factory.Create();
      var command = sqlGenerator.GetCommand(selectExpression);

      string sql = command.CommandText;
      return sql;
    }

    [SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "<Pendente>")]
    private static object Private(this object obj, string privateField) =>
      obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);

    [SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "<Pendente>")]
    private static T Private<T>(this object obj, string privateField) =>
      (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
  }
}
