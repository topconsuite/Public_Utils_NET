using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Lists;

namespace Telluria.Utils.Crud.Repositories
{
  public interface IBaseCrudRepository<TEntity>
    where TEntity : BaseEntity
  {
    Task Commit(CancellationToken cancellationToken);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
    Task AddAsync<TSpecificEntity>(IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
      where TSpecificEntity : BaseEntity;

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
    Task UpdateAsync<TSpecificEntity>(IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
      where TSpecificEntity : BaseEntity;

    Task UpsertAsync(
      TEntity entity,
      Expression<Func<TEntity, object>> match,
      Expression<Func<TEntity, TEntity, TEntity>> updater,
      CancellationToken cancellationToken);
    Task UpsertAsync<TSpecificEntity>(
      TEntity entity,
      Expression<Func<TEntity, object>> match,
      Expression<Func<TEntity, TEntity, TEntity>> updater,
      CancellationToken cancellationToken)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    /// "Soft Delete": set the property "Deleted" to TRUE.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void Task.</returns>
    Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// "Soft Delete": set the property "Deleted" to TRUE.
    /// </summary>
    /// <param name="entities">Entity/Entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void Task.</returns>
    Task SoftDeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    /// <summary>
    /// "Soft Delete": set the property "Deleted" to TRUE.
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="entities">Entity/Entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void Task.</returns>
    Task SoftDeleteAsync<TSpecificEntity>(IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    /// Remove the registry permanently from the database.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void Task.</returns>
    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Remove the registry permanently from the database.
    /// </summary>
    /// <param name="entities">Entity/Entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void Task.</returns>
    Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    /// <summary>
    /// Remove the registry permanently from the database.
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="entities">Entity/Entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void Task.</returns>
    Task RemoveAsync<TSpecificEntity>(IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    /// Return a register that matches the given ID not deleted (when using "Soft Delete").
    /// </summary>
    /// <param name="id">ID of the requested resource.</param>
    /// <param name="tracking">Determines if EntityFramework will track changes on the result.</param>
    /// <param name="includeProperties">List of strings containing the names of child properties to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task of TEntity.</returns>
    Task<TEntity> GetAsync(
      Guid id,
      bool tracking,
      IEnumerable<string> includeProperties,
      CancellationToken cancellationToken);

    /// <summary>
    /// Return a register that matches the given ID not deleted (when using "Soft Delete").
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="id">ID of the requested resource.</param>
    /// <param name="tracking">Determines if EntityFramework will track changes on the result.</param>
    /// <param name="includeProperties">List of strings containing the names of child properties to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task of TSpecificEntity.</returns>
    Task<TSpecificEntity> GetAsync<TSpecificEntity>(
      Guid id,
      bool tracking,
      IEnumerable<string> includeProperties,
      CancellationToken cancellationToken)
        where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return first or default register (that matches the filter) not deleted (when using "Soft Delete").
    /// </summary>
    /// <param name="tracking">Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">List of strings containing the names of child properties to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task of TEntity.</returns>
    Task<TEntity> FindAsync(
      bool tracking,
      Expression<Func<TEntity, bool>> filter,
      IEnumerable<string> includeProperties,
      CancellationToken cancellationToken);

    /// <summary>
    ///     Return first or default register (that matches the filter) not deleted (when using "Soft Delete").
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="tracking">Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">List of strings containing the names of child properties to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task of TSpecificEntity.</returns>
    Task<TSpecificEntity> FindAsync<TSpecificEntity>(
      bool tracking,
      Expression<Func<TSpecificEntity, bool>> filter,
      IEnumerable<string> includeProperties,
      CancellationToken cancellationToken)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return all registers (that matches the filter) not deleted (when using "Soft Delete").
    /// </summary>
    /// <param name="page">Page number.</param>
    /// <param name="perPage">Number of registers per page.</param>
    /// <param name="tracking">Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">List of strings containing the names of child properties to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>TEntity PagedList Task.</returns>
    Task<PagedList<TEntity>> ListAsync(
      uint page,
      uint perPage,
      bool tracking,
      Expression<Func<TEntity, bool>> filter,
      IEnumerable<string> includeProperties,
      CancellationToken cancellationToken);

    /// <summary>
    ///     Return all registers (that matches the filter) not deleted (when using "Soft Delete").
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="page">Page number.</param>
    /// <param name="perPage">Number of registers per page.</param>
    /// <param name="tracking">Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">List of strings containing the names of child properties to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>TSpecificEntity PagedList Task.</returns>
    Task<PagedList<TSpecificEntity>> ListAsync<TSpecificEntity>(
      uint page,
      uint perPage,
      bool tracking,
      Expression<Func<TSpecificEntity, bool>> filter,
      IEnumerable<string> includeProperties,
      CancellationToken cancellationToken)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return all registers (that matches the filter) not deleted (when using "Soft Delete") without pagination.
    /// </summary>
    /// <param name="tracking">Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">List of strings containing the names of child properties to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>TEntity IEnumerable Task.</returns>
    Task<IEnumerable<TEntity>> ListAsync(
      bool tracking,
      Expression<Func<TEntity, bool>> filter,
      IEnumerable<string> includeProperties,
      CancellationToken cancellationToken);

    /// <summary>
    /// Return all registers (that matches the filter) not deleted (when using "Soft Delete") without pagination.
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="tracking">Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">List of strings containing the names of child properties to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>TSpecificEntity IEnumerable Task.</returns>
    Task<IEnumerable<TSpecificEntity>> ListAsync<TSpecificEntity>(
      bool tracking,
      Expression<Func<TSpecificEntity, bool>> filter,
      IEnumerable<string> includeProperties,
      CancellationToken cancellationToken)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return all registers (that matches the filter), including deleted ones (when using "Soft Delete").
    /// </summary>
    /// <param name="page">Page number.</param>
    /// <param name="perPage">Number of registers per page.</param>
    /// <param name="tracking">Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">List of strings containing the names of child properties to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>TEntity PagedList Task.</returns>
    Task<PagedList<TEntity>> ListAllAsync(
      uint page,
      uint perPage,
      bool tracking,
      Expression<Func<TEntity, bool>> filter,
      IEnumerable<string> includeProperties,
      CancellationToken cancellationToken);

    /// <summary>
    ///     Return all registers (that matches the filter), including deleted ones (when using "Soft Delete").
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="page">Page number.</param>
    /// <param name="perPage">Number of registers per page.</param>
    /// <param name="tracking">Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">List of strings containing the names of child properties to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>TSpecificEntity PagedList Task.</returns>
    Task<PagedList<TSpecificEntity>> ListAllAsync<TSpecificEntity>(
      uint page,
      uint perPage,
      bool tracking,
      Expression<Func<TSpecificEntity, bool>> filter,
      IEnumerable<string> includeProperties,
      CancellationToken cancellationToken)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return all registers (that matches the filter), including deleted ones (when using "Soft Delete") without pagination.
    /// </summary>
    /// <param name="tracking">Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">List of strings containing the names of child properties to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>TEntity IEnumerable Task.</returns>
    Task<IEnumerable<TEntity>> ListAllAsync(
      bool tracking,
      Expression<Func<TEntity, bool>> filter,
      IEnumerable<string> includeProperties,
      CancellationToken cancellationToken);

    /// <summary>
    ///     Return all registers (that matches the filter), including deleted ones (when using "Soft Delete") without pagination.
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="tracking">Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">List of strings containing the names of child properties to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>TSpecificEntity IEnumerable Task.</returns>
    Task<IEnumerable<TSpecificEntity>> ListAllAsync<TSpecificEntity>(
      bool tracking,
      Expression<Func<TSpecificEntity, bool>> filter,
      IEnumerable<string> includeProperties,
      CancellationToken cancellationToken)
      where TSpecificEntity : BaseEntity;
  }
}
