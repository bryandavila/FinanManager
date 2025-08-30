using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class StatusPresupuesto
{
  public int StatusId { get; set; }
  public string StatusName { get; set; } = null!;

  // Propiedad de navegación
  public virtual ICollection<Bienes> Bienes { get; set; } = new List<Bienes>();
  public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
  public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
}
