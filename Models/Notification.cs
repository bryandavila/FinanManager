using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class Notification
{
    public int NotificationsId { get; set; }

    public string NotificationType { get; set; } = null!;

    public string NotificationMessage { get; set; } = null!;

    public int? NotTriggeredBy { get; set; }

    public virtual User? NotTriggeredByNavigation { get; set; }
}
