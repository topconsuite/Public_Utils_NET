using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.GraphQL.InputTypes;
using Telluria.Utils.Crud.Lists;

namespace Telluria.Utils.Crud.Repositories;

[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "More readable")]
public interface IBaseCrudRepository<TEntity>
  where TEntity : BaseEntity
{
  Task Commit(CancellationToken cancellationToken);

  #region CREATE (ADD)

  Task AddAsync(TEntity entity, CancellationToken cancellationToken);
  Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

  Task AddAsync<TSpecificEntity>(IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity;

  #endregion
  #region READ (GET BY ID)

  Task<TEntity> GetAsync(
    Guid id, bool tracking, IEnumerable<string> includeProperties, CancellationToken cancellationToken);

  Task<TSpecificEntity> GetAsync<TSpecificEntity>(
    Guid id, bool tracking, IEnumerable<string> includeProperties, CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity;

  #endregion
  #region READ (GET BY FILTER - ONE REGISTER)

  Task<TEntity> FindAsync(
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken);

  Task<TSpecificEntity> FindAsync<TSpecificEntity>(
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity;

  #endregion
  #region READ (GET BY FILTER - MULTIPLE REGISTERS)

  Task<IEnumerable<TEntity>> ListAsync(
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken);

  Task<IEnumerable<TSpecificEntity>> ListAsync<TSpecificEntity>(
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity;

  #endregion
  #region READ (WITH PAGINATION - EXCLUDE SOFT DELETED)

  Task<PagedList<TEntity>> ListAsync(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken);

  Task<PagedList<TSpecificEntity>> ListAsync<TSpecificEntity>(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity;

  #endregion
  #region READ (WITH PAGINATION - INCLUDE SOFT DELETED)

  Task<PagedList<TEntity>> ListAllAsync(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken);

  Task<PagedList<TSpecificEntity>> ListAllAsync<TSpecificEntity>(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity;

  #endregion
  #region READ (WITHOUT PAGINATION - INCLUDE SOFT DELETED)

  Task<IEnumerable<TEntity>> ListAllAsync(
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken);

  Task<IEnumerable<TSpecificEntity>> ListAllAsync<TSpecificEntity>(
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity;

  #endregion
  #region READ (WITH PAGINATION AND SORTING - EXCLUDE SOFT DELETED)

  Task<PagedList<TEntity>> ListSortedAsync(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    SortClauses[] sort,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken);

  Task<PagedList<TSpecificEntity>> ListSortedAsync<TSpecificEntity>(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    SortClauses[] sort,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity;

  #endregion
  #region READ (WITH PAGINATION AND SORTING - INCLUDE SOFT DELETED)

  Task<PagedList<TEntity>> ListAllSortedAsync(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TEntity, bool>> filter,
    SortClauses[] sort,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken);

  Task<PagedList<TSpecificEntity>> ListAllSortedAsync<TSpecificEntity>(
    uint page,
    uint perPage,
    bool tracking,
    Expression<Func<TSpecificEntity, bool>> filter,
    SortClauses[] sort,
    IEnumerable<string> includeProperties,
    CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity;

  #endregion
  #region UPDATE

  Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
  Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

  Task UpdateAsync<TSpecificEntity>(IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity;

  #endregion
  #region UPDATE (SOFT DELETE)

  Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken);
  Task SoftDeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

  Task SoftDeleteAsync<TSpecificEntity>(IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity;

  #endregion
  #region UPSERT

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

  #endregion
  #region DELETE

  Task RemoveAsync(TEntity entity, CancellationToken cancellationToken);
  Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

  Task RemoveAsync<TSpecificEntity>(IEnumerable<TSpecificEntity> entities, CancellationToken cancellationToken)
    where TSpecificEntity : BaseEntity;

  #endregion
}
