using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Telluria.Utils.Crud.Services;

public interface ITenantService
{
  Guid TenantId { get; set; }
  bool HasRequest { get; set; }
  void ApplyTenantFilterConfigurations(ModelBuilder modelBuilder);
  void SetEntityTenant(EntityEntry entry);
}
