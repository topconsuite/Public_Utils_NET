using System.Collections.Generic;
using System.Linq;

namespace Telluria.Utils.Crud.DTOs.Validation
{
  public abstract class Notifiable
  {
    public IReadOnlyCollection<FluentValidation.Results.ValidationFailure> Notifications { get; private set; }
    public bool IsValid { get => (Notifications?.Count() ?? 0) == 0; }

    public void AddNotifications(params FluentValidation.Results.ValidationFailure[] items)
    {
      Notifications = Notifications ?? new List<FluentValidation.Results.ValidationFailure>();
      foreach (var item in items)
      {
        Notifications.Append(item);
      }
    }

    public void ClearNotifications()
    {
      Notifications = null;
    }
  }
}
