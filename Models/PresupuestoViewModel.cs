namespace FinanManager.Models
{
  public class PresupuestoViewModel
  {
    public int Id { get; set; }
    public string Tipo { get; set; }
    public string Descripcion { get; set; }
    public DateTime Fecha { get; set; }
    public int Estado { get; set; }
  }
}
