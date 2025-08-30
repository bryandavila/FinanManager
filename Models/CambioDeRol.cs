using System;
using System.Collections.Generic;

namespace FinanManager.Models
{
  public class CambioDeRol
  {
    public int Id { get; set; }
    public string Email { get; set; }
    public string RolAnterior { get; set; }
    public string NuevoRol { get; set; }
    public DateTime FechaCambio { get; set; }
    public string Responsable { get; set; }
  }
}
