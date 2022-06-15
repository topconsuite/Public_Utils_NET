using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Lists;

namespace Telluria.Utils.Crud.Repositories
{
  public interface IBaseCrudRepository<TEntity>
    where TEntity : BaseEntity
  {
    Task Commit();

    Task AddAsync(params TEntity[] entities);
    Task AddAsync<TSpecificEntity>(params TSpecificEntity[] entities)
      where TSpecificEntity : BaseEntity;

    Task UpdateAsync(params TEntity[] entities);
    Task UpdateAsync<TSpecificEntity>(params TSpecificEntity[] entities)
      where TSpecificEntity : BaseEntity;

    Task UpsertAsync(
      TEntity entity,
      Expression<Func<TEntity, object>> match,
      Expression<Func<TEntity, TEntity, TEntity>> updater);
    Task UpsertAsync<TSpecificEntity>(
      TEntity entity,
      Expression<Func<TEntity, object>> match,
      Expression<Func<TEntity, TEntity, TEntity>> updater)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    /// "Soft Delete": set the property "Deleted" to TRUE.
    /// </summary>
    /// <param name="entities">Entity/Entities.</param>
    /// <returns>Void Task.</returns>
    Task SoftDeleteAsync(params TEntity[] entities);

    /// <summary>
    /// "Soft Delete": set the property "Deleted" to TRUE.
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="entities">Entity/Entities.</param>
    /// <returns>Void Task.</returns>
    Task SoftDeleteAsync<TSpecificEntity>(params TSpecificEntity[] entities)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    /// Remove the registry permanently from the database.
    /// </summary>
    /// <param name="entities">Entity/Entities.</param>
    /// <returns>Void Task.</returns>
    Task RemoveAsync(params TEntity[] entities);

    /// <summary>
    /// Remove the registry permanently from the database.
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="entities">Entity/Entities.</param>
    /// <returns>Void Task.</returns>
    Task RemoveAsync<TSpecificEntity>(params TSpecificEntity[] entities)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    /// Return a register that matches the given ID not deleted (when using "Soft Delete").
    /// </summary>
    /// <param name="id">ID of the requested resource.</param>
    /// <param name="tracking">(Optional, default = false) Determines if EntityFramework will track changes on the result.</param>
    /// <param name="includeProperties">(Optional) Array of strings containing the names of child properties to include.</param>
    /// <returns>Task of TEntity.</returns>
    Task<TEntity> GetAsync(
      Guid id,
      bool tracking = false,
      params string[] includeProperties);

    /// <summary>
    /// Return a register that matches the given ID not deleted (when using "Soft Delete").
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="id">ID of the requested resource.</param>
    /// <param name="tracking">(Optional, default = false) Determines if EntityFramework will track changes on the result.</param>
    /// <param name="includeProperties">(Optional) Array of strings containing the names of child properties to include.</param>
    /// <returns>Task of TSpecificEntity.</returns>
    Task<TSpecificEntity> GetAsync<TSpecificEntity>(
      Guid id,
      bool tracking = false,
      params string[] includeProperties)
        where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return first or default register (that matches the filter) not deleted (when using "Soft Delete").
    /// </summary>
    /// <param name="tracking">(Optional, default = false) Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">(Optional) Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">(Optional) Array of strings containing the names of child properties to include.</param>
    /// <returns>Task of TEntity.</returns>
    Task<TEntity> FindAsync(
      bool tracking = false,
      Expression<Func<TEntity, bool>> filter = null,
      params string[] includeProperties);

    /// <summary>
    ///     Return first or default register (that matches the filter) not deleted (when using "Soft Delete").
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="tracking">(Optional, default = false) Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">(Optional) Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">(Optional) Array of strings containing the names of child properties to include.</param>
    /// <returns>Task of TSpecificEntity.</returns>
    Task<TSpecificEntity> FindAsync<TSpecificEntity>(
      bool tracking = false,
      Expression<Func<TSpecificEntity, bool>> filter = null,
      params string[] includeProperties)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return all registers (that matches the filter) not deleted (when using "Soft Delete").
    /// </summary>
    /// <param name="page">Page number.</param>
    /// <param name="perPage">Number of registers per page.</param>
    /// <param name="tracking">(Optional, default = false) Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">(Optional) Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">(Optional) Array of strings containing the names of child properties to include.</param>
    /// <returns>TEntity PagedList Task.</returns>
    Task<PagedList<TEntity>> ListAsync(
      uint page,
      uint perPage,
      bool tracking = false,
      Expression<Func<TEntity, bool>> filter = null,
      params string[] includeProperties);

    /// <summary>
    ///     Return all registers (that matches the filter) not deleted (when using "Soft Delete").
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="page">Page number.</param>
    /// <param name="perPage">Number of registers per page.</param>
    /// <param name="tracking">(Optional, default = false) Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">(Optional) Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">(Optional) Array of strings containing the names of child properties to include.</param>
    /// <returns>TSpecificEntity PagedList Task.</returns>
    Task<PagedList<TSpecificEntity>> ListAsync<TSpecificEntity>(
      uint page,
      uint perPage,
      bool tracking = false,
      Expression<Func<TSpecificEntity, bool>> filter = null,
      params string[] includeProperties)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return all registers (that matches the filter) not deleted (when using "Soft Delete") without pagination.
    /// </summary>
    /// <param name="tracking">(Optional, default = false) Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">(Optional) Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">(Optional) Array of strings containing the names of child properties to include.</param>
    /// <returns>TEntity IEnumerable Task.</returns>
    Task<IEnumerable<TEntity>> ListAsync(
      bool tracking = false,
      Expression<Func<TEntity, bool>> filter = null,
      params string[] includeProperties);

    /// <summary>
    /// Return all registers (that matches the filter) not deleted (when using "Soft Delete") without pagination.
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="tracking">(Optional, default = false) Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">(Optional) Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">(Optional) Array of strings containing the names of child properties to include.</param>
    /// <returns>TSpecificEntity IEnumerable Task.</returns>
    Task<IEnumerable<TSpecificEntity>> ListAsync<TSpecificEntity>(
      bool tracking = false,
      Expression<Func<TSpecificEntity, bool>> filter = null,
      params string[] includeProperties)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return all registers (that matches the filter), including deleted ones (when using "Soft Delete").
    /// </summary>
    /// <param name="page">Page number.</param>
    /// <param name="perPage">Number of registers per page.</param>
    /// <param name="tracking">(Optional, default = false) Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">(Optional) Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">(Optional) Array of strings containing the names of child properties to include.</param>
    /// <returns>TEntity PagedList Task.</returns>
    Task<PagedList<TEntity>> ListAllAsync(
      uint page,
      uint perPage,
      bool tracking = false,
      Expression<Func<TEntity, bool>> filter = null,
      params string[] includeProperties);

    /// <summary>
    ///     Return all registers (that matches the filter), including deleted ones (when using "Soft Delete").
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="page">Page number.</param>
    /// <param name="perPage">Number of registers per page.</param>
    /// <param name="tracking">(Optional, default = false) Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">(Optional) Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">(Optional) Array of strings containing the names of child properties to include.</param>
    /// <returns>TSpecificEntity PagedList Task.</returns>
    Task<PagedList<TSpecificEntity>> ListAllAsync<TSpecificEntity>(
      uint page,
      uint perPage,
      bool tracking = false,
      Expression<Func<TSpecificEntity, bool>> filter = null,
      params string[] includeProperties)
      where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return all registers (that matches the filter), including deleted ones (when using "Soft Delete") without pagination.
    /// </summary>
    /// <param name="tracking">(Optional, default = false) Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">(Optional) Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">(Optional) Array of strings containing the names of child properties to include.</param>
    /// <returns>TEntity IEnumerable Task.</returns>
    Task<IEnumerable<TEntity>> ListAllAsync(
      bool tracking = false,
      Expression<Func<TEntity, bool>> filter = null,
      params string[] includeProperties);

    /// <summary>
    ///     Return all registers (that matches the filter), including deleted ones (when using "Soft Delete") without pagination.
    /// </summary>
    /// <typeparam name="TSpecificEntity">Specific Entity Type.</typeparam>
    /// <param name="tracking">(Optional, default = false) Determines if EntityFramework will track changes on the result.</param>
    /// <param name="filter">(Optional) Expression that will be converted in WHERE clause on database query.</param>
    /// <param name="includeProperties">(Optional) Array of strings containing the names of child properties to include.</param>
    /// <returns>TSpecificEntity IEnumerable Task.</returns>
    Task<IEnumerable<TSpecificEntity>> ListAllAsync<TSpecificEntity>(
      bool tracking = false,
      Expression<Func<TSpecificEntity, bool>> filter = null,
      params string[] includeProperties)
      where TSpecificEntity : BaseEntity;
  }
}
