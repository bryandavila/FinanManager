namespace FinanManager.Models
{
  public class BienAprobadoBase
  {
    public string Descripcion { get; set; }
    public DateTime Fecha { get; set; }
    public string RoleName { get; set; }
  }

  public class BienesAprobados : BienAprobadoBase
  {
    public int Cantidad { get; set; }
    public decimal MontoUnitario { get; set; }
    public decimal Total { get; set; }
    public string MotivoRechazo { get; set; }
  }

  public class GastoAprobado : BienAprobadoBase
  {
    public string CuentaMadre_ID { get; set; }
    public string Justificacion { get; set; }
    public decimal Total { get; set; }
  }

  public class ProyectoAprobado : BienAprobadoBase
  {
    public decimal ValorEstimado { get; set; }
    public bool VialidadComercial { get; set; }
    public bool VialidadTecnica { get; set; }
    public bool VialidadLegal { get; set; }
    public bool VialidadGestion { get; set; }
    public bool VialidadFinanciera { get; set; }
  }
}
