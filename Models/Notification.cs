using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanManager.Models;

public partial class Notification
{
  [Key]
  [Column("Notifications_ID")] // Especifica el nombre de la columna en la base de datos
  public int NotificationsId { get; set; }

  public string NotificationType { get; set; } = null!;

  public string NotificationMessage { get; set; } = null!;

  [Column("role_ID")] // Clave externa que referencia a la tabla Roles
  public int role_ID { get; set; }

  // Propiedad de navegación para la relación con Roles
  public virtual Role? Role { get; set; }
}
