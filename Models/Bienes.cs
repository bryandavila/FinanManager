using FinanManager.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinanManager.Models;
public partial class Bienes
{
  public int BienId { get; set; }

  [JsonPropertyName("descripcionBien")]
  public string Descripcion { get; set; } = null!;

  public int Cantidad { get; set; }

  public decimal MontoUnitario { get; set; }

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

  public DateTime Fecha { get; set; }

  [Column("status_ID")]
  public int StatusId { get; set; }

  [JsonPropertyName("motivoRechazo")]
  public string? MotivoRechazo { get; set; }

  // Propiedad de navegación (no requerida)
  public virtual Role? Role { get; set; }

  // Propiedad de navegación para el estado
  public virtual StatusPresupuesto? StatusPresupuesto { get; set; }
}
