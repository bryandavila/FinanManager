using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanManager.Models;
using System.Security.Claims;
using System.Linq;

namespace FinanManager.Controllers
{
  public class VerPresupuestoController : Controller
  {
    private readonly FinanManagerContext _context;

    public VerPresupuestoController(FinanManagerContext context)
    {
      _context = context;
    }

    // Acción Index para mostrar la lista de presupuestos
    public IActionResult Index()
    {
      // Obtener el valor del claim "RoleID"
      var roleClaimValue = User.FindFirstValue("RoleID");

      // Verificar si el claim existe y es un número válido
      if (string.IsNullOrEmpty(roleClaimValue) || !int.TryParse(roleClaimValue, out var roleId))
      {
        return RedirectToAction("Error", "Home");
      }

      // Obtener los presupuestos (Bienes, Gastos y Proyectos) filtrados por el rol del usuario
      var presupuestos = _context.Bienes
          .Where(b => b.RoleId == roleId)
          .Select(b => new PresupuestoViewModel
          {
            Id = b.BienId,
            Tipo = "Bien", // Puedes asignar un valor fijo o manejarlo de otra manera
            Descripcion = b.Descripcion,
            Fecha = b.Fecha,
            Estado = b.StatusId
          })
          .Union(_context.Gasto
              .Where(g => g.RoleId == roleId)
              .Select(g => new PresupuestoViewModel
              {
                Id = g.GastoId,
                Tipo = "Gasto",
                Descripcion = g.Justificacion,
                Fecha = g.Fecha,
                Estado = g.StatusId
              }))
          .Union(_context.Proyectos
              .Where(p => p.RoleId == roleId)
              .Select(p => new PresupuestoViewModel
              {
                Id = p.ProyectoId,
                Tipo = "Proyecto",
                Descripcion = p.Descripcion, // Valor por defecto para proyectos
                Fecha = p.Fecha,
                Estado = p.StatusId
              }))
          .ToList();

      return View(presupuestos);
    }

    // Acción Detalles para mostrar los detalles de un presupuesto específico
    public IActionResult Detalles(int id, string tipo)
    {
      var roleClaimValue = User.FindFirstValue("RoleID");

      if (string.IsNullOrEmpty(roleClaimValue) || !int.TryParse(roleClaimValue, out var roleId))
      {
        return RedirectToAction("Error", "Home");
      }

      object presupuesto = null;

      switch (tipo)
      {
        case "Bien":
          presupuesto = _context.Bienes
              .Include(b => b.StatusPresupuesto) // Usa la propiedad de navegación correcta
              .FirstOrDefault(b => b.BienId == id && b.RoleId == roleId);
          break;
        case "Gasto":
          presupuesto = _context.Gasto
              .Include(g => g.Status)
              .FirstOrDefault(g => g.GastoId == id && g.RoleId == roleId);
          break;
        case "Proyecto":
          presupuesto = _context.Proyectos
              .Include(p => p.Status)
              .FirstOrDefault(p => p.ProyectoId == id && p.RoleId == roleId);
          break;
        default:
          return NotFound();
      }

      if (presupuesto == null)
      {
        return NotFound();
      }

      return View(presupuesto);
    }
  }
}
