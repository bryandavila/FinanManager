using Microsoft.AspNetCore.Mvc;
using FinanManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace FinanManager.Controllers
{
  public class GastosController : Controller
  {
    private readonly FinanManagerContext _context;

    public GastosController(FinanManagerContext context)
    {
      _context = context;
    }

    // Acción para mostrar el resumen de gastos
    public IActionResult ResumenGastos()
    {
      // Obtener los gastos agrupados por CuentaMadreId
      var gastosPorCategoria = _context.Gasto
          .Include(g => g.Role)
          .Include(g => g.Status)
          .GroupBy(g => g.CuentaMadreId)
          .Select(g => new
          {
            Categoria = g.Key,
            Gastos = g.ToList()
          })
          .ToList();

      // Pasar los datos a la vista
      return View("~/Views/ResumenGastos/Index.cshtml", gastosPorCategoria);
    }
  }
}
