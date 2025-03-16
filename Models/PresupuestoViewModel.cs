namespace FinanManager.Models
{
  public class PresupuestoViewModel
  {
    // Campos comunes
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public int Estado { get; set; }
    public string? MotivoRechazo { get; set; }
    public decimal Monto { get; set; }
    public string RolNombre { get; set; }

    // Campos específicos para Bienes
    public decimal Cantidad { get; set; }
    public decimal MontoUnitario { get; set; }

    // Campos específicos para Gastos
    public string Justificacion { get; set; } = string.Empty;

    // Campos específicos para Proyectos
    public decimal ValorEstimado { get; set; }
    public string ViabilidadComercial { get; set; } = string.Empty;
    public string ViabilidadTecnica { get; set; } = string.Empty;
    public string ViabilidadLegal { get; set; } = string.Empty;
    public string ViabilidadGestion { get; set; } = string.Empty;
    public string ViabilidadImpactoAmbiental { get; set; } = string.Empty;
    public string ViabilidadFinanciera { get; set; } = string.Empty;

    // Campos para el periodo de ejecución (solo para Bienes y Gastos)
    public bool Enero { get; set; }
    public bool Febrero { get; set; }
    public bool Marzo { get; set; }
    public bool Abril { get; set; }
    public bool Mayo { get; set; }
    public bool Junio { get; set; }
    public bool Julio { get; set; }
    public bool Agosto { get; set; }
    public bool Septiembre { get; set; }
    public bool Octubre { get; set; }
    public bool Noviembre { get; set; }
    public bool Diciembre { get; set; }
  }
}
