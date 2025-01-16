// ReSharper disable PossibleMultipleEnumeration

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telluria.Utils.Crud.Constants.Enums;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.GraphQL.InputTypes;
using Telluria.Utils.Crud.Helpers;
using Telluria.Utils.Crud.Lists;
using Telluria.Utils.Crud.Services;

namespace Telluria.Utils.Crud.Repositories;

[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "More readable")]
public abstract class BaseCrudRepository<TEntity> : IBaseCrudRepository<TEntity>
  where TEntity : BaseEntity
{
  protected readonly IServiceProvider _serviceProvider;
  protected readonly ITenantService _tenantService;
  protected readonly DbContext _context;

  protected BaseCrudRepository(IServiceProvider provider, DbContext context)
  {
    _serviceProvider = provider;

    _tenantService = provider.GetRequiredService<ITenantService>();

    _context = context;
  }

  /// <summary>
  ///   Commit changes to the database.
  /// </summary>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
  /// <returns> Integer of number of the state entries written to the database. </returns>
  public async Task Commit(CancellationToken cancellationToken)
  {
    Exception shouldThrow = null;

    try
    {
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception e)
    {
      await _context.DisposeAsync();
      shouldThrow = e;
    }

    if (shouldThrow != null) throw shouldThrow;
  }

  /// <summary>
  ///   Set the entity to be used in the database.
  /// </summary>
  /// <typeparam name="TSpecificEntity"> Type of the entity. </typeparam>
  /// <returns> A set for the given entity type. </returns>
  private DbSet<TSpecificEntity> DbSet<TSpecificEntity>()
    where TSpecificEntity : BaseEntity
  {
    return _context.Set<TSpecificEntity>();
  }

  /// <summary>
  ///   Validates if the entities exist in the database.
  /// </summary>
  /// <param name="entities"> The entities to be validated. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <typeparam name="TSpecificEntity"> Type of the entity. </typeparam>
  /// <exception cref="Exception"> Thrown when an exception error condition occurs. (Custom not found exception). </exception>
  private async Task ValidateExistence<TSpecificEntity>(
    IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity
  {
    if (entities.Any(entity => entity.Id == Guid.Empty))
      throw new Exception($"Id '{Guid.Empty}' not found");

    var entitiesIds = entities.Select(x => x.Id);
    var oldEntities = await ListAsync<TSpecificEntity>(false, x => entitiesIds.Contains(x.Id), null, cancellationToken);

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

  #region CREATE (ADD)

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  ///   PS: Just one entity.
  /// </summary>
  /// <param name="entity"> Entity. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> The new entity. </returns>
  public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
  {
    await AddAsync<TEntity>(new[] { entity }, cancellationToken);
  }

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  ///   PS: Multiple entities.
  /// </summary>
  /// <param name="entities"> Entities. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> The new entity. </returns>
  public virtual async Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
  {
    await AddAsync<TEntity>(entities, cancellationToken);
  }

  /// <summary>
  ///   Add entities to database.
  /// </summary>
  /// <param name="entities"> Entities. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <typeparam name="TSpecificEntity"> Type of entity. </typeparam>
  /// <returns> The new entity. </returns>
  public virtual async Task AddAsync<TSpecificEntity>(
    IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity
  {
    var now = DateTime.Now.ToUniversalTime();

    foreach (var entity in entities)
    {
      entity.CreatedAt = now;
      entity.UpdatedAt = now;
      entity.DeletedAt = null;
      entity.Deleted = false;
    }

    await DbSet<TSpecificEntity>().AddRangeAsync(entities, cancellationToken);
  }

  #endregion
  #region READ (GET BY ID)

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  /// </summary>
  /// <param name="id"> ID of entity. </param>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> The found entity. </returns>
  public virtual async Task<TEntity> GetAsync(
    Guid id, bool tracking, IEnumerable<string> includeProperties, CancellationToken cancellationToken)
  {
    return await GetAsync<TEntity>(id, tracking, includeProperties, cancellationToken);
  }

  /// <summary>
  ///   Get entity by id
  ///   Ignore soft deleted entities.
  /// </summary>
  /// <param name="id"> ID of entity. </param>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <typeparam name="TSpecificEntity"> Type of entity. </typeparam>
  /// <returns> The found entity. </returns>
  public virtual async Task<TSpecificEntity> GetAsync<TSpecificEntity>(
    Guid id, bool tracking, IEnumerable<string> includeProperties, CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity
  {
    var set = DbSet<TSpecificEntity>().AsQueryable();

    set = set.AddIncludes(includeProperties);
    set = set.Where(t => t.Id == id);

    return await set.Tracking(tracking).FirstOrDefaultAsync(cancellationToken);
  }

  #endregion
  #region READ (GET BY FILTER - ONE REGISTER)

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  /// </summary>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter expression. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> The found entity. </returns>
  public virtual async Task<TEntity> FindAsync(
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
  {
    return await FindAsync<TEntity>(tracking, filter, includeProperties, cancellationToken);
  }

  /// <summary>
  ///   Get entity by filter (without pagination).
  ///   Ignore soft deleted entities.
  ///   Only one entity.
  /// </summary>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter expression. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <typeparam name="TSpecificEntity"> Specific entity. </typeparam>
  /// <returns> The found entity. </returns>
  public virtual async Task<TSpecificEntity> FindAsync<TSpecificEntity>(
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity
  {
    var set = DbSet<TSpecificEntity>().AsQueryable();

    set = set.AddIncludes(includeProperties);

    if (filter != null)
      set = set.Where(filter);

    return await set.Tracking(tracking).FirstOrDefaultAsync(cancellationToken);
  }

  #endregion
  #region READ (GET BY FILTER - MULTIPLE REGISTERS)

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  /// </summary>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter expression. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <param name="ignoreQueryFilters"> Iguinore query filters. </param>
  /// <returns> The found entities. </returns>
  public virtual async Task<IEnumerable<TEntity>> ListAsync(
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken,
    bool ignoreQueryFilters = false)
  {
    return await ListAsync<TEntity>(tracking, filter, includeProperties, cancellationToken, ignoreQueryFilters);
  }

  /// <summary>
  ///   Get entity by filter (without pagination).
  ///   Ignore soft deleted entities.
  ///   Multiple entities.
  /// </summary>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter expression. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <param name="ignoreQueryFilters"> Iguinore query filters. </param>
  /// <typeparam name="TSpecificEntity"> Specific entity. </typeparam>
  /// <returns> The found entities. </returns>
  public virtual async Task<IEnumerable<TSpecificEntity>> ListAsync<TSpecificEntity>(
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken,
    bool ignoreQueryFilters = false)
    where TSpecificEntity : BaseEntity
  {
    var set = DbSet<TSpecificEntity>().AsQueryable();

    set = set.AddIncludes(includeProperties);

    if (filter != null)
      set = set.Where(filter);

    if (ignoreQueryFilters)
      return await set.IgnoreQueryFilters().Tracking(tracking).ToListAsync(cancellationToken);

    return await set.Tracking(tracking).ToListAsync(cancellationToken);
  }

  #endregion
  #region READ (WITH PAGINATION - EXCLUDE SOFT DELETED)

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  /// </summary>
  /// <param name="page"> Page number. </param>
  /// <param name="perPage"> Number of items per page. </param>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter expression. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> Paged list of entities. </returns>
  public virtual async Task<PagedList<TEntity>> ListAsync(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
  {
    return await ListAsync<TEntity>(
      page, perPage, tracking, filter, includeProperties, cancellationToken);
  }

  /// <summary>
  ///   Get a list of entities (with pagination).
  ///   Ignore soft deleted entities.
  /// </summary>
  /// <param name="page"> Page number. </param>
  /// <param name="perPage"> Number of items per page. </param>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter expression. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <typeparam name="TSpecificEntity"> Type of entity. </typeparam>
  /// <returns> Paged list of entities. </returns>
  public virtual async Task<PagedList<TSpecificEntity>> ListAsync<TSpecificEntity>(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity
  {
    var set = DbSet<TSpecificEntity>().AsQueryable();

    set = set.AddIncludes(includeProperties);

    if (filter != null)
      set = set.Where(filter);

    return await set.PagedList(page, perPage, tracking, cancellationToken);
  }

  #endregion
  #region READ (WITH PAGINATION - INCLUDE SOFT DELETED)

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  /// </summary>
  /// <param name="page"> Page number. </param>
  /// <param name="perPage"> Number of items per page. </param>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter expression. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> Paged list of entities. </returns>
  public virtual async Task<PagedList<TEntity>> ListAllAsync(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
  {
    return await ListAllAsync<TEntity>(
      page, perPage, tracking, filter, includeProperties, cancellationToken);
  }

  /// <summary>
  ///   Get a list of entities (with pagination).
  ///   Include soft deleted entities.
  /// </summary>
  /// <param name="page"> Page number. </param>
  /// <param name="perPage"> Number of items per page. </param>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter expression. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <typeparam name="TSpecificEntity"> Type of entity. </typeparam>
  /// <returns> Paged list of entities. </returns>
  public virtual async Task<PagedList<TSpecificEntity>> ListAllAsync<TSpecificEntity>(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity
  {
    var set = DbSet<TSpecificEntity>().AsQueryable();

    set = set.AddIncludes(includeProperties);

    if (filter != null)
      set = set.Where(filter);

    return await set.PagedList(page, perPage, tracking, cancellationToken);
  }

  #endregion
  #region READ (WITHOUT PAGINATION - INCLUDE SOFT DELETED)

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  /// </summary>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> List of entities. </returns>
  public virtual async Task<IEnumerable<TEntity>> ListAllAsync(
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
  {
    return await ListAllAsync<TEntity>(tracking, filter, includeProperties, cancellationToken);
  }

  /// <summary>
  ///   Get a list of entities (without pagination).
  ///   Include soft deleted entities.
  /// </summary>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <typeparam name="TSpecificEntity"> Type of entity. </typeparam>
  /// <returns> List of entities. </returns>
  public virtual async Task<IEnumerable<TSpecificEntity>> ListAllAsync<TSpecificEntity>(
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity
  {
    var set = DbSet<TSpecificEntity>().AsQueryable();

    set = set.AddIncludes(includeProperties);

    if (filter != null)
      set = set.Where(filter);

    return await set.Tracking(tracking).ToListAsync(cancellationToken);
  }

  #endregion
  #region READ (WITH PAGINATION AND SORTING - EXCLUDE SOFT DELETED)

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  /// </summary>
  /// <param name="page"> Page number. </param>
  /// <param name="perPage"> Number of items per page. </param>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter expression. </param>
  /// <param name="sort"> Sort expression. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> Paged list of entities. </returns>
  public virtual async Task<PagedList<TEntity>> ListSortedAsync(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    SortClauses[] sort,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
  {
    return await ListSortedAsync<TEntity>(page, perPage, tracking, filter, sort, includeProperties, cancellationToken);
  }

  /// <summary>
  ///   Get a list of entities (with pagination and sorting).
  /// </summary>
  /// <param name="page"> Page number. </param>
  /// <param name="perPage"> Number of items per page. </param>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter expression. </param>
  /// <param name="sort"> Sort expression. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <typeparam name="TSpecificEntity"> Type of entity. </typeparam>
  /// <returns> Paged list of entities. </returns>
  public virtual async Task<PagedList<TSpecificEntity>> ListSortedAsync<TSpecificEntity>(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    SortClauses[] sort,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity
  {
    var set = DbSet<TSpecificEntity>().AsQueryable();

    set = set.AddIncludes(includeProperties);

    if (filter != null)
      set = set.Where(filter);

    var orderedQuery = set.OrderByDescending(x => x.CreatedAt);

    // Verify if sort is null, if it is, use default sort (CreatedAt descending).
    if (sort is not { Length: > 0 })
      return await orderedQuery.PagedList(page, perPage, tracking, cancellationToken);

    for (var i = 0; i < sort.Length; i++)
    {
      var clause = sort[i];
      var desc = clause.SortDirection == ESort.DESC;

      if (i == 0)
        orderedQuery = set.OrderBy(clause.Field, desc);
      else
        orderedQuery = orderedQuery.ThenBy(clause.Field, desc);
    }

    return await orderedQuery.PagedList(page, perPage, tracking, cancellationToken);
  }

  #endregion
  #region READ (WITH PAGINATION AND SORTING - INCLUDE SOFT DELETED)

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  /// </summary>
  /// <param name="page"> Page number. </param>
  /// <param name="perPage"> Number of items per page. </param>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter expression. </param>
  /// <param name="sort"> Sort expression. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> Paged list of entities. </returns>
  public virtual async Task<PagedList<TEntity>> ListAllSortedAsync(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    SortClauses[] sort,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
  {
    return await ListAllSortedAsync<TEntity>(
      page, perPage, tracking, filter, sort, includeProperties, cancellationToken);
  }

  /// <summary>
  ///   Get a list of entities (with pagination and sorting).
  ///   Include soft deleted entities.
  /// </summary>
  /// <param name="page"> Page number. </param>
  /// <param name="perPage"> Number of items per page. </param>
  /// <param name="tracking"> Tracking or not. </param>
  /// <param name="filter"> Filter expression. </param>
  /// <param name="sort"> Sort expression. </param>
  /// <param name="includeProperties"> Include properties. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <typeparam name="TSpecificEntity"> Type of entity. </typeparam>
  /// <returns> Paged list of entities. </returns>
  public virtual async Task<PagedList<TSpecificEntity>> ListAllSortedAsync<TSpecificEntity>(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    SortClauses[] sort,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity
  {
    var set = DbSet<TSpecificEntity>().AsQueryable();

    set = set.AddIncludes(includeProperties);

    if (filter != null)
      set = set.Where(filter);

    var orderedQuery = set.OrderByDescending(x => x.CreatedAt);

    // Verify if sort is null, if it is, use default sort (CreatedAt descending).
    if (sort is not { Length: > 0 })
      return await orderedQuery.PagedList(page, perPage, tracking, cancellationToken);

    for (var i = 0; i < sort.Length; i++)
    {
      var clause = sort[i];
      var desc = clause.SortDirection == ESort.DESC;

      if (i == 0)
        orderedQuery = set.OrderBy(clause.Field, desc);
      else
        orderedQuery = orderedQuery.ThenBy(clause.Field, desc);
    }

    return await orderedQuery.PagedList(page, perPage, tracking, cancellationToken);
  }

  #endregion
  #region UPDATE

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  ///   PS: Just one entity.
  /// </summary>
  /// <param name="entity"> Entity. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> Task. </returns>
  public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
  {
    await UpdateAsync<TEntity>(new[] { entity }, cancellationToken);
  }

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  ///   PS: Multiple entities.
  /// </summary>
  /// <param name="entities"> Entities. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> Task. </returns>
  public virtual async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
  {
    await UpdateAsync<TEntity>(entities, cancellationToken);
  }

  /// <summary>
  ///   Update entity or entities
  ///   Ignore soft deleted entities.
  /// </summary>
  /// <param name="entities"> Entities. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <typeparam name="TSpecificEntity"> Type of entity. </typeparam>
  /// <returns> Task. </returns>
  public virtual async Task UpdateAsync<TSpecificEntity>(
    IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity
  {
    await ValidateExistence(entities, cancellationToken);

    var now = DateTime.Now.ToUniversalTime();

    foreach (var entity in entities)
      entity.UpdatedAt = now;

    await DbSet<TSpecificEntity>().UpdateRangeAsync(entities, cancellationToken);
  }

  #endregion
  #region UPDATE (SOFT DELETE)

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  ///   PS: Just one entity.
  /// </summary>
  /// <param name="entity"> Entity. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> Task. </returns>
  public virtual async Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken)
  {
    await SoftDeleteAsync<TEntity>(new[] { entity }, cancellationToken);
  }

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  ///   PS: Multiple entities.
  /// </summary>
  /// <param name="entities"> Entities. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> Task. </returns>
  public virtual async Task SoftDeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
  {
    await SoftDeleteAsync<TEntity>(entities, cancellationToken);
  }

  /// <summary>
  ///   Soft delete entities.
  ///   Update DeletedAt and Deleted fields.
  /// </summary>
  /// <param name="entities"> Entities. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <typeparam name="TSpecificEntity"> Specific entity. </typeparam>
  /// <returns> Task. </returns>
  public virtual async Task SoftDeleteAsync<TSpecificEntity>(
    IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity
  {
    await ValidateExistence(entities, cancellationToken);

    var now = DateTime.Now.ToUniversalTime();

    foreach (var entity in entities)
    {
      entity.DeletedAt = now;
      entity.Deleted = true;
    }

    // Verify if have foreign key conflicts to continue or not with soft delete
    using (new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
    {
      await DbSet<TSpecificEntity>().RemoveRangeAsync(entities, cancellationToken);
      await Commit(cancellationToken);
    }

    await DbSet<TSpecificEntity>().UpdateRangeAsync(entities, cancellationToken);
  }

  #endregion
  #region DELETE

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  ///   PS: Just one entity.
  /// </summary>
  /// <param name="entity"> Entity. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> Task. </returns>
  public virtual async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
  {
    await RemoveAsync<TEntity>(new[] { entity }, cancellationToken);
  }

  /// <summary>
  ///   TRANSFER
  ///   Calls the function responsible for implementation to ensure correct required inheritance.
  ///   PS: Multiple entities.
  /// </summary>
  /// <param name="entities"> Entities. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <returns> Task. </returns>
  public virtual async Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
  {
    await RemoveAsync<TEntity>(entities, cancellationToken);
  }

  /// <summary>
  ///   Deletes the entity from the database.
  /// </summary>
  /// <param name="entities"> Entities. </param>
  /// <param name="cancellationToken"> Cancellation token. </param>
  /// <typeparam name="TSpecificEntity"> Entity type. </typeparam>
  /// <returns> Task. </returns>
  public virtual async Task RemoveAsync<TSpecificEntity>(
    IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity
  {
    await DbSet<TSpecificEntity>().RemoveRangeAsync(entities, cancellationToken);
  }

  #endregion
}
