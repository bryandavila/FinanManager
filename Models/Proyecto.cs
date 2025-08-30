using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class Proyecto
{
  public int ProyectoId { get; set; }
  public decimal ValorEstimado { get; set; }
  public string Descripcion { get; set; } = null!;
  public string? ViabilidadComercial { get; set; }
  public string? ViabilidadTecnica { get; set; }
  public string? ViabilidadLegal { get; set; }
  public string? ViabilidadGestion { get; set; }
  public string? ViabilidadImpactoAmbiental { get; set; }
  public string? ViabilidadFinanciera { get; set; }
  public int RoleId { get; set; }
  public int StatusId { get; set; }
  public DateTime Fecha { get; set; }
  public string? MotivoRechazo { get; set; }
  public virtual Role? Role { get; set; } = null!;
  public virtual StatusPresupuesto? Status { get; set; } = null!;
}
