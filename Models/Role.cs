using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class Role
{
  public int role_ID { get; set; }
  public string role_name { get; set; } = null!;

  public virtual ICollection<Bienes> Bienes { get; set; } = new List<Bienes>();
  public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
  public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
  public virtual ICollection<User> Users { get; set; } = new List<User>();
}
