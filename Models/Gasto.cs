using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class Gasto
{
    public int GastoId { get; set; }
    public int CuentaMadreId { get; set; }
    public int CuentaHijaId { get; set; }
    public string Justificacion { get; set; } = null!;
    public decimal Total { get; set; }
    public int Enero { get; set; }
    public int Febrero { get; set; }
    public int Marzo { get; set; }
    public int Abril { get; set; }
    public int Mayo { get; set; }
    public int Junio { get; set; }
    public int Julio { get; set; }
    public int Agosto { get; set; }
    public int Septiembre { get; set; }
    public int Octubre { get; set; }
    public int Noviembre { get; set; }
    public int Diciembre { get; set; }
    public int RoleId { get; set; }
    public int StatusId { get; set; }
    public DateTime Fecha { get; set; }
    public string? MotivoRechazo { get; set; }
    public virtual Role? Role { get; set; } = null!;
    public virtual StatusPresupuesto? Status { get; set; } = null!;
}
