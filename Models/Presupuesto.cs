using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinanManager.Models;

public partial class Presupuesto
{
    public int PresupuestoId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

  public virtual User CreatedByNavigation { get; set; } = null!;
}
