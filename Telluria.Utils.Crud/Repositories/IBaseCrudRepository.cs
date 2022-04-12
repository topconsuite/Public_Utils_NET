using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Lists;

namespace Telluria.Utils.Crud.Repositories
{
  public interface IBaseCrudRepository<TEntity> where TEntity : BaseEntity
  {
    Task Commit();
    Task AddAsync(params TEntity[] entities);
    Task AddAsync<TSpecificEntity>(params TSpecificEntity[] entities) where TSpecificEntity : BaseEntity;

    Task UpdateAsync(params TEntity[] entities);
    Task UpdateAsync<TSpecificEntity>(params TSpecificEntity[] entities) where TSpecificEntity : BaseEntity;

    Task UpsertAsync(TEntity entity, Expression<Func<TEntity, object>> match, Expression<Func<TEntity, TEntity, TEntity>> updater);
    Task UpsertAsync<TSpecificEntity>(TEntity entity, Expression<Func<TEntity, object>> match, Expression<Func<TEntity, TEntity, TEntity>> updater) where TSpecificEntity : BaseEntity;

    /// <summary>
    /// "Soft Delete": set the property "Deleted" to TRUE.
    /// </summary>
    Task SoftDeleteAsync(params TEntity[] entities);
    Task SoftDeleteAsync<TSpecificEntity>(params TSpecificEntity[] entities) where TSpecificEntity : BaseEntity;

    /// <summary>
    /// Remove the registry permanently from the database.
    /// </summary>
    Task RemoveAsync(params TEntity[] entities);
    Task RemoveAsync<TSpecificEntity>(params TSpecificEntity[] entities) where TSpecificEntity : BaseEntity;

    Task<TEntity> GetAsync(Guid id, bool tracking = false, params string[] includeProperties);
    Task<TSpecificEntity> GetAsync<TSpecificEntity>(Guid id, bool tracking = false, params string[] includeProperties)
        where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return first or default register (that matches the filter) not deleted (when using "Soft Delete").
    /// </summary>
    Task<TEntity> FindAsync(bool tracking = false,
        Expression<Func<TEntity, bool>> filter = null,
        params string[] includeProperties);
    /// <summary>
    ///     Return first or default register (that matches the filter) not deleted (when using "Soft Delete").
    /// </summary>
    Task<TSpecificEntity> FindAsync<TSpecificEntity>(bool tracking = false,
        Expression<Func<TSpecificEntity, bool>> filter = null,
        params string[] includeProperties) where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return all registers (that matches the filter) not deleted (when using "Soft Delete").
    /// </summary>
    Task<PagedList<TEntity>> ListAsync(uint page, uint perPage, bool tracking = false,
        Expression<Func<TEntity, bool>> filter = null,
        params string[] includeProperties);
    /// <summary>
    ///     Return all registers (that matches the filter) not deleted (when using "Soft Delete").
    /// </summary>
    Task<PagedList<TSpecificEntity>> ListAsync<TSpecificEntity>(uint page, uint perPage, bool tracking = false,
        Expression<Func<TSpecificEntity, bool>> filter = null,
        params string[] includeProperties) where TSpecificEntity : BaseEntity;

    /// <summary>
    ///     Return all registers (that matches the filter), including deleted ones (when using "Soft Delete").
    /// </summary>
    Task<PagedList<TEntity>> ListAllAsync(uint page, uint perPage, bool tracking = false,
        Expression<Func<TEntity, bool>> filter = null,
        params string[] includeProperties);
    /// <summary>
    ///     Return all registers (that matches the filter), including deleted ones (when using "Soft Delete").
    /// </summary>
    Task<PagedList<TSpecificEntity>> ListAllAsync<TSpecificEntity>(uint page, uint perPage, bool tracking = false,
        Expression<Func<TSpecificEntity, bool>> filter = null,
        params string[] includeProperties) where TSpecificEntity : BaseEntity;
  }
}
