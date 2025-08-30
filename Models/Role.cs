using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinanManager.Models;

public partial class Role
{
  [Key]
  [Column("role_ID")] // Mapea a role_ID
  public int role_ID { get; set; }

  [Column("role_name")] // Mapea a role_name
  public string role_name { get; set; } = null!;

  public virtual ICollection<Bienes> Bienes { get; set; } = new List<Bienes>();
  public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
  public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
  public virtual ICollection<User> Users { get; set; } = new List<User>();
  public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
  public virtual ICollection<AlertConfig> AlertConfigs { get; set; } = new List<AlertConfig>();
}
