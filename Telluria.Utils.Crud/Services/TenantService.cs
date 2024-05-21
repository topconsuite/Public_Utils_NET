using System;

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
}
