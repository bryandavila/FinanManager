using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanManager.Models;
using System.Linq;
using System.Security.Claims;

[AuthorizeRole(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21)]  // roles 2 al 21
public class GestionPresupuestoController : Controller
{
  private readonly FinanManagerContext _context;

  public GestionPresupuestoController(FinanManagerContext context)
  {
    _context = context;
  }

  public IActionResult Index()
  {
    // Obtener el RoleId del usuario actual
    var roleClaimValue = User.FindFirstValue("RoleID");
    if (string.IsNullOrEmpty(roleClaimValue) || !int.TryParse(roleClaimValue, out var roleId))
    {
      return RedirectToAction("Error", "Home");
    }

    // Obtener presupuestos pendientes de aprobación
    var presupuestosPendientes = _context.Bienes
        .Where(b => b.RoleId == roleId && b.StatusId == 1) // 1 = Pendiente
        .Select(b => new PresupuestoViewModel
        {
          Id = b.BienId,
          Tipo = "Bien",
          Descripcion = b.Descripcion,
          Fecha = b.Fecha,
          Estado = b.StatusId,
          Monto = b.Total
        })
        .Union(_context.Gasto
            .Where(g => g.RoleId == roleId && g.StatusId == 1)
            .Select(g => new PresupuestoViewModel
            {
              Id = g.GastoId,
              Tipo = "Gasto",
              Descripcion = g.Justificacion,
              Fecha = g.Fecha,
              Estado = g.StatusId,
              Monto = g.Total
            }))
        .Union(_context.Proyectos
            .Where(p => p.RoleId == roleId && p.StatusId == 1)
            .Select(p => new PresupuestoViewModel
            {
              Id = p.ProyectoId,
              Tipo = "Proyecto",
              Descripcion = p.Descripcion,
              Fecha = p.Fecha,
              Estado = p.StatusId,
              Monto = p.ValorEstimado
            }))
        .ToList();

    // Obtener presupuestos rechazados  test
    var presupuestosRechazados = _context.Bienes
        .Where(b => b.RoleId == roleId && b.StatusId == 3) // 3 = Rejected
        .Select(b => new PresupuestoViewModel
        {
          Id = b.BienId,
          Tipo = "Bien",
          Descripcion = b.Descripcion,
          Fecha = b.Fecha,
          Estado = b.StatusId,
          MotivoRechazo = b.MotivoRechazo
        })
        .Union(_context.Gasto
            .Where(g => g.RoleId == roleId && g.StatusId == 3)
            .Select(g => new PresupuestoViewModel
            {
              Id = g.GastoId,
              Tipo = "Gasto",
              Descripcion = g.Justificacion,
              Fecha = g.Fecha,
              Estado = g.StatusId,
              MotivoRechazo = g.MotivoRechazo
            }))
        .Union(_context.Proyectos
            .Where(p => p.RoleId == roleId && p.StatusId == 3)
            .Select(p => new PresupuestoViewModel
            {
              Id = p.ProyectoId,
              Tipo = "Proyecto",
              Descripcion = p.Descripcion,
              Fecha = p.Fecha,
              Estado = p.StatusId,
              MotivoRechazo = p.MotivoRechazo
            }))
        .ToList();

    // Pasar los datos a la vista
    ViewBag.PresupuestosPendientes = presupuestosPendientes;
    ViewBag.PresupuestosRechazados = presupuestosRechazados;

    return View();
  }
}
