using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanManager.Models;
using System.Linq;

namespace FinanManager.Controllers
{
  public class RevisarPresupuestoController : Controller
  {
    private readonly FinanManagerContext _context;

    public RevisarPresupuestoController(FinanManagerContext context)
    {
      _context = context;
    }

    // Acción Index para mostrar la lista de presupuestos pendientes (estado 2)
    public IActionResult Index(string filtroRol)
    {
      // Obtener todos los presupuestos (Bienes, Gastos y Proyectos) con estado 2 (Pendiente)
      var presupuestosQuery = _context.Bienes
          .Include(b => b.Role) // Incluir la propiedad de navegación Role
          .Where(b => b.StatusId == 2) // Solo estado 2 (Pendiente)
          .Select(b => new PresupuestoViewModel
          {
            Id = b.BienId,
            Tipo = "Bien",
            Descripcion = b.Descripcion,
            Fecha = b.Fecha,
            Estado = b.StatusId,
            Monto = b.Total,
            MotivoRechazo = b.MotivoRechazo,
            RolNombre = b.Role.role_name // Incluir el nombre del rol
          })
          .AsEnumerable() // Convertir a enumerable para evitar problemas con Union
          .Union(_context.Gasto
              .Include(g => g.Role) // Incluir la propiedad de navegación Role
              .Where(g => g.StatusId == 2) // Solo estado 2 (Pendiente)
              .Select(g => new PresupuestoViewModel
              {
                Id = g.GastoId,
                Tipo = "Gasto",
                Descripcion = g.Justificacion,
                Fecha = g.Fecha,
                Estado = g.StatusId,
                Monto = g.Total,
                MotivoRechazo = g.MotivoRechazo,
                RolNombre = g.Role.role_name // Incluir el nombre del rol
              })
              .AsEnumerable()) // Convertir a enumerable para evitar problemas con Union
          .Union(_context.Proyectos
              .Include(p => p.Role) // Incluir la propiedad de navegación Role
              .Where(p => p.StatusId == 2) // Solo estado 2 (Pendiente)
              .Select(p => new PresupuestoViewModel
              {
                Id = p.ProyectoId,
                Tipo = "Proyecto",
                Descripcion = p.Descripcion,
                Fecha = p.Fecha,
                Estado = p.StatusId,
                Monto = p.ValorEstimado,
                MotivoRechazo = p.MotivoRechazo,
                RolNombre = p.Role.role_name // Incluir el nombre del rol
              })
              .AsEnumerable()); // Convertir a enumerable para evitar problemas con Union

      // Aplicar filtro por nombre de rol si se proporciona
      if (!string.IsNullOrEmpty(filtroRol))
      {
        presupuestosQuery = presupuestosQuery
            .Where(p => p.RolNombre.Contains(filtroRol, StringComparison.OrdinalIgnoreCase));
      }

      var presupuestos = presupuestosQuery.ToList();

      // Pasar la lista de roles disponibles a la vista para el dropdown
      var roles = _context.Roles
          .Select(r => r.role_name)
          .Distinct()
          .ToList();

      ViewBag.Roles = roles; // Pasar los roles a la vista
      ViewBag.FiltroRol = filtroRol; // Pasar el filtro actual a la vista

      return View(presupuestos);
    }

    // Acción Detalles para mostrar la información completa de un presupuesto
    public IActionResult Detalles(int id, string tipo)
    {
      object presupuesto = null;

      switch (tipo)
      {
        case "Bien":
          presupuesto = _context.Bienes
              .Include(b => b.Role) // Incluir el rol
              .FirstOrDefault(b => b.BienId == id && b.StatusId == 2); // Solo estado 2 (Pendiente)
          break;
        case "Gasto":
          presupuesto = _context.Gasto
              .Include(g => g.Role) // Incluir el rol
              .FirstOrDefault(g => g.GastoId == id && g.StatusId == 2); // Solo estado 2 (Pendiente)
          break;
        case "Proyecto":
          presupuesto = _context.Proyectos
              .Include(p => p.Role) // Incluir el rol
              .FirstOrDefault(p => p.ProyectoId == id && p.StatusId == 2); // Solo estado 2 (Pendiente)
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

    // Acción para aprobar un presupuesto
    [HttpPost]
    public IActionResult Aprobar([FromBody] AprobarRechazarRequest request)
    {
      try
      {
        switch (request.Tipo)
        {
          case "Bien":
            var bien = _context.Bienes.FirstOrDefault(b => b.BienId == request.Id);
            if (bien != null)
            {
              bien.StatusId = 3; // 3 = Aprobado
              _context.Bienes.Update(bien);
              _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_Bienes_Notifications ON Bienes");
              _context.SaveChanges();
              _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_Bienes_Notifications ON Bienes");
              // Forzar la ejecución del TRIGGER con un UPDATE dummy
              _context.Database.ExecuteSqlRaw("UPDATE Bienes SET status_ID = status_ID WHERE bien_ID = @p0", request.Id);
              TempData["Mensaje"] = "El bien ha sido aprobado correctamente.";
            }
            break;
          case "Gasto":
            var gasto = _context.Gasto.FirstOrDefault(g => g.GastoId == request.Id);
            if (gasto != null)
            {
              gasto.StatusId = 3; // 3 = Aprobado
              _context.Gasto.Update(gasto);
              _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_InsertNotification ON Gasto");
              _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_Gasto_Notifications ON Gasto");
              _context.SaveChanges();
              _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_InsertNotification ON Gasto");
              _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_Gasto_Notifications ON Gasto");
              // Forzar la ejecución del TRIGGER con un UPDATE dummy
              _context.Database.ExecuteSqlRaw("UPDATE Gasto SET status_ID = status_ID WHERE gasto_ID = @p0", request.Id);
              TempData["Mensaje"] = "El gasto ha sido aprobado correctamente.";
            }
            break;
          case "Proyecto":
            var proyecto = _context.Proyectos.FirstOrDefault(p => p.ProyectoId == request.Id);
            if (proyecto != null)
            {
              proyecto.StatusId = 3; // 3 = Aprobado
              _context.Proyectos.Update(proyecto);
              _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_Proyectos_Notifications ON Proyectos");
              _context.SaveChanges();
              _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_Proyectos_Notifications ON Proyectos");
              // Forzar la ejecución del TRIGGER con un UPDATE dummy
              _context.Database.ExecuteSqlRaw("UPDATE Proyectos SET status_ID = status_ID WHERE proyecto_ID = @p0", request.Id);
              TempData["Mensaje"] = "El proyecto ha sido aprobado correctamente.";
            }
            break;
          default:
            return Json(new { success = false, message = "Tipo de presupuesto no válido." });
        }

        return Json(new { success = true, message = TempData["Mensaje"] });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Ocurrió un error al aprobar el presupuesto." });
      }
    }

    // Acción para rechazar un presupuesto
    [HttpPost]
    public IActionResult Rechazar([FromBody] AprobarRechazarRequest request)
    {
      try
      {
        switch (request.Tipo)
        {
          case "Bien":
            var bien = _context.Bienes.FirstOrDefault(b => b.BienId == request.Id);
            if (bien != null)
            {
              bien.StatusId = 1; // 1 = Rechazado
              bien.MotivoRechazo = request.MotivoRechazo;
              _context.Bienes.Update(bien);
              _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_Bienes_Notifications ON Bienes");
              _context.SaveChanges();
              _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_Bienes_Notifications ON Bienes");
              // Forzar la ejecución del TRIGGER con un UPDATE dummy
              _context.Database.ExecuteSqlRaw("UPDATE Bienes SET status_ID = status_ID WHERE bien_ID = @p0", request.Id);

              TempData["Mensaje"] = "El bien ha sido rechazado correctamente.";
            }
            break;
          case "Gasto":
            var gasto = _context.Gasto.FirstOrDefault(g => g.GastoId == request.Id);
            if (gasto != null)
            {
              gasto.StatusId = 1; // 1 = Rechazado
              gasto.MotivoRechazo = request.MotivoRechazo;
              _context.Gasto.Update(gasto);
              _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_InsertNotification ON Gasto");
              _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_Gasto_Notifications ON Gasto");
              _context.SaveChanges();
              _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_InsertNotification ON Gasto");
              _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_Gasto_Notifications ON Gasto");
              // Forzar la ejecución del TRIGGER con un UPDATE dummy
              _context.Database.ExecuteSqlRaw("UPDATE Gasto SET status_ID = status_ID WHERE gasto_ID = @p0", request.Id);
              TempData["Mensaje"] = "El gasto ha sido rechazado correctamente.";
            }
            break;
          case "Proyecto":
            var proyecto = _context.Proyectos.FirstOrDefault(p => p.ProyectoId == request.Id);
            if (proyecto != null)
            {
              proyecto.StatusId = 1; // 1 = Rechazado
              proyecto.MotivoRechazo = request.MotivoRechazo;
              _context.Proyectos.Update(proyecto);
              _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_Proyectos_Notifications ON Proyectos");
              _context.SaveChanges();
              _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_Proyectos_Notifications ON Proyectos");
              // Forzar la ejecución del TRIGGER con un UPDATE dummy
              _context.Database.ExecuteSqlRaw("UPDATE Proyectos SET status_ID = status_ID WHERE proyecto_ID = @p0", request.Id);
              TempData["Mensaje"] = "El proyecto ha sido rechazado correctamente.";
            }
            break;
          default:
            return Json(new { success = false, message = "Tipo de presupuesto no válido." });
        }

        return Json(new { success = true, message = TempData["Mensaje"] });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Ocurrió un error al rechazar el presupuesto." });
      }
    }
  }

  public class AprobarRechazarRequest
  {
    public int Id { get; set; }
    public string Tipo { get; set; }
    public string MotivoRechazo { get; set; }
  }
}
