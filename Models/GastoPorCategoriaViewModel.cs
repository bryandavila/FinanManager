namespace FinanManager.Models
{
  public class GastoPorCategoriaViewModel
  {
    public int Categoria { get; set; } // CuentaMadreId
    public string NombreCategoria { get; set; } // Nombre de la categoria
    public int CuentaHijaId { get; set; } // CuentaHijaId
    public string NombreCuentaHija { get; set; } // Nombre de la CuentaHija
    public List<Gasto> Gastos { get; set; } // Lista de gastos
  }
}
