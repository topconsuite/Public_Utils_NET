using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Helpers;

namespace Telluria.Utils.Crud.Services;

/// <summary>
/// Serviço responsavem pelo gerenciamento do Tenant
/// Esse serviço deve ser instanciado somente com ciclo de vida scoped, pois deve criar apenas uma instancia a cada request.
/// </summary>
public class TenantService : ITenantService
{
  public Guid TenantId { get; set; }
  public bool HasRequest { get; set; }

  public TenantService(bool hasQuery = true)
  {
    HasRequest = hasQuery;
  }

  public void ApplyTenantFilterConfigurations(ModelBuilder modelBuilder)
  {
    var entities = modelBuilder.Model.GetEntityTypes().Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType));

    entities.ForEach(entityType =>
    {
      var parameter = Expression.Parameter(entityType.ClrType, "e");

      // Cria uma expressão para e.TenantId == _tenantService.TenantId
      var tenantIdProperty = Expression.Property(parameter, nameof(BaseEntity.TenantId));
      var tenantIdValue = Expression.Constant(TenantId);
      var tenantIdEqual = Expression.Equal(tenantIdProperty, tenantIdValue);

      // Cria uma expressão para !e.Deleted
      var deletedProperty = Expression.Property(parameter, nameof(BaseEntity.Deleted));
      var notDeleted = Expression.Not(deletedProperty);

      // Combina as duas expressões com AND
      var andExpression = Expression.AndAlso(tenantIdEqual, notDeleted);

      var lambda = Expression.Lambda(andExpression, parameter);

      modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
    });
  }

  public void SetEntityTenant(EntityEntry entry)
  {
    if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
    {
      var tenantId = entry.Property("TenantId").CurrentValue as Guid?;

      // Se tenantId for nulo ou vazio, indica erro
      if (TenantId == Guid.Empty)
        throw new InvalidOperationException("TenantId is required and cannot be empty.");

      // Se tenantId tiver o valor deve ser igual ao fornecido pelo tenantService
      if (tenantId.HasValue && tenantId.Value != Guid.Empty && tenantId.Value != TenantId)
        throw new InvalidOperationException("The provided TenantId does not match the expected TenantId from tenant service.");

      // Atribue o tennat somente se não possuir o tennat
      if (tenantId == Guid.Empty || !tenantId.HasValue)
        entry.Property("TenantId").CurrentValue = TenantId;
    }
  }
}
