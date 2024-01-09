namespace Telluria.Utils.Crud.Sample;

public interface ITenantProvider
{
  string TenantId { get; }
}

public sealed class TenantProvider : ITenantProvider
{
  private const string TenantIdHeaderName = "tenantId";
  private readonly IHttpContextAccessor _httpContextAccessor;

  public TenantProvider(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public string TenantId => _httpContextAccessor
    .HttpContext!
    .Request
    .Headers[TenantIdHeaderName];
}
