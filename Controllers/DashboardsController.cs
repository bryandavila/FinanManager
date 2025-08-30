using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanManager.Models;
using System.Linq;
using System.Threading.Tasks;

namespace FinanManager.Controllers
{
  [AuthorizeRole(2)]
  public class DashboardsController : Controller
  {
    private readonly FinanManagerContext _context;

    public DashboardsController(FinanManagerContext context)
    {
      _context = context;
    }

    public async Task<IActionResult> Index()
    {
      // Filtrar datos con StatusId = 3
      var bienes = await _context.Bienes.Include(b => b.Role).Where(b => b.StatusId == 3).ToListAsync();
      var gastos = await _context.Gasto.Where(g => g.StatusId == 3).ToListAsync();
      var proyectos = await _context.Proyectos.Where(p => p.StatusId == 3).ToListAsync();
      var roles = await _context.Roles.Include(r => r.Bienes).ToListAsync();

      decimal totalPresupuesto = bienes.Sum(b => b.Total) + proyectos.Sum(p => p.ValorEstimado);
      decimal totalGastos = gastos.Sum(g => g.Total);

      // Evitar división por cero
      decimal porcentajeGastos = totalPresupuesto > 0 ? (totalGastos / totalPresupuesto) * 100 : 0;

      // Distribución por rol
      var distribucionPresupuesto = roles.ToDictionary(
          role => role.role_name,
          role => bienes.Where(b => b.RoleId == role.role_ID).Sum(b => b.Total)
      );

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

      ViewBag.TotalPresupuesto = totalPresupuesto;
      ViewBag.TotalGastos = totalGastos;
      ViewBag.PorcentajeGastos = porcentajeGastos;
      ViewBag.DistribucionPresupuesto = distribucionPresupuesto;
      ViewBag.TendenciaGastos = tendenciaGastos;
      ViewBag.Roles = roles.Select(r => r.role_name).ToList();

      return View();
    }
  }
}
