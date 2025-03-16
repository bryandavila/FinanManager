using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanManager.Models;
using System.Linq;
using System.Threading.Tasks;

namespace FinanManager.Controllers
{
  public class DashboardsController : Controller
  {
    private readonly FinanManagerContext _context;

    public DashboardsController(FinanManagerContext context)
    {
      _context = context;
    }

    public async Task<IActionResult> Index()
    {
      // Obtener datos de Bienes, Gastos, Proyectos y Roles
      var bienes = await _context.Bienes.Include(b => b.Role).ToListAsync();
      var gastos = await _context.Gasto.ToListAsync();
      var proyectos = await _context.Proyectos.ToListAsync();
      var roles = await _context.Roles.Include(r => r.Bienes).ToListAsync();

      // Calcular totales y distribuciones
      var totalPresupuesto = bienes.Sum(b => b.Total) + proyectos.Sum(p => p.ValorEstimado);
      var totalGastos = gastos.Sum(g => g.Total);

      // Calcular la distribución del presupuesto por rol
      var distribucionPresupuesto = roles.ToDictionary(
          role => role.role_name, // Nombre del rol como clave
          role => bienes.Where(b => b.RoleId == role.role_ID).Sum(b => b.Total) // Suma de los bienes por rol
      );

      // Calcular la tendencia de gastos por mes
      var tendenciaGastos = new
      {
        Enero = gastos.Sum(g => g.Enero),
        Febrero = gastos.Sum(g => g.Febrero),
        Marzo = gastos.Sum(g => g.Marzo),
        Abril = gastos.Sum(g => g.Abril),
        Mayo = gastos.Sum(g => g.Mayo),
        Junio = gastos.Sum(g => g.Junio),
        Julio = gastos.Sum(g => g.Julio),
        Agosto = gastos.Sum(g => g.Agosto),
        Septiembre = gastos.Sum(g => g.Septiembre),
        Octubre = gastos.Sum(g => g.Octubre),
        Noviembre = gastos.Sum(g => g.Noviembre),
        Diciembre = gastos.Sum(g => g.Diciembre)
      };

      // Pasar datos a la vista
      ViewBag.TotalPresupuesto = totalPresupuesto;
      ViewBag.TotalGastos = totalGastos;
      ViewBag.DistribucionPresupuesto = distribucionPresupuesto;
      ViewBag.TendenciaGastos = tendenciaGastos;
      ViewBag.Roles = roles.Select(r => r.role_name).ToList(); // Lista de nombres de roles

      return View();
    }
  }
}
