namespace Telluria.Utils.Crud.Entities
{
  public class IntegrationMessage
  {
    public string Table { get; set; }
    public string Entity { get; set; }
    public string Action { get; set; }
    public string TenantId { get; set; }
    public object Content { get; set; }
  }
}
