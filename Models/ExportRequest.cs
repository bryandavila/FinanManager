namespace FinanManager.Models
{
  public class ExportRequest
  {
    public List<string> Categories { get; set; } // Filtro por categorías (pueden ser IDs de cuentas)
    public DateTime StartDate { get; set; } // Fecha de inicio
    public DateTime EndDate { get; set; } // Fecha de fin
    public string Notes { get; set; } // Notas adicionales
    public List<string> Columns { get; set; } // Columnas a incluir en el reporte
  }
}
