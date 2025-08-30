using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanManager.Models;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;

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
      var roleClaimValue = User.FindFirstValue("RoleID");

      if (string.IsNullOrEmpty(roleClaimValue) || !int.TryParse(roleClaimValue, out var roleId))
      {
        return RedirectToAction("Error", "Home");
      }

      // Obtener todos los presupuestos (Bienes, Gastos y Proyectos) filtrados por el rol del usuario
      var presupuestos = _context.Bienes
          .Where(b => b.RoleId == roleId)
          .Select(b => new PresupuestoViewModel
          {
            Id = b.BienId,
            Tipo = "Bien",
            Descripcion = b.Descripcion,
            Fecha = b.Fecha,
            Estado = b.StatusId,
            Monto = b.Total,
            MotivoRechazo = b.MotivoRechazo
          })
          .Union(_context.Gasto
              .Where(g => g.RoleId == roleId)
              .Select(g => new PresupuestoViewModel
              {
                Id = g.GastoId,
                Tipo = "Gasto",
                Descripcion = g.Justificacion,
                Fecha = g.Fecha,
                Estado = g.StatusId,
                Monto = g.Total,
                MotivoRechazo = g.MotivoRechazo
              }))
          .Union(_context.Proyectos
              .Where(p => p.RoleId == roleId)
              .Select(p => new PresupuestoViewModel
              {
                Id = p.ProyectoId,
                Tipo = "Proyecto",
                Descripcion = p.Descripcion,
                Fecha = p.Fecha,
                Estado = p.StatusId,
                Monto = p.ValorEstimado,
                MotivoRechazo = p.MotivoRechazo
              }))
          .ToList();

      // Obtener presupuestos rechazados
      var presupuestosRechazados = presupuestos
          .Where(p => p.Estado == 1) // 1 = Rechazado
          .ToList();

      ViewBag.PresupuestosRechazados = presupuestosRechazados;

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
              .Include(b => b.StatusPresupuesto)
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

    // Acción para mostrar presupuestos rechazados
    public IActionResult Rechazados()
    {
      var roleClaimValue = User.FindFirstValue("RoleID");

      if (string.IsNullOrEmpty(roleClaimValue) || !int.TryParse(roleClaimValue, out var roleId))
      {
        return RedirectToAction("Error", "Home");
      }

      // Obtener todos los presupuestos rechazados (Bienes, Gastos y Proyectos)
      var presupuestosRechazados = _context.Bienes
          .Where(b => b.RoleId == roleId && b.StatusId == 1) // 1 = Rechazado
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
              .Where(g => g.RoleId == roleId && g.StatusId == 1)
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
              .Where(p => p.RoleId == roleId && p.StatusId == 1)
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

      return View(presupuestosRechazados);
    }

    // Acción GET para mostrar el formulario de edición
    public IActionResult EditarRechazado(int id, string tipo)
    {
      var roleClaimValue = User.FindFirstValue("RoleID");

      if (string.IsNullOrEmpty(roleClaimValue) || !int.TryParse(roleClaimValue, out var roleId))
      {
        return RedirectToAction("Error", "Home");
      }

      PresupuestoViewModel presupuestoViewModel = null;

      switch (tipo)
      {
        case "Bien":
          var bien = _context.Bienes
              .Include(b => b.StatusPresupuesto)
              .FirstOrDefault(b => b.BienId == id && b.RoleId == roleId);
          if (bien != null)
          {
            presupuestoViewModel = new PresupuestoViewModel
            {
              Id = bien.BienId,
              Tipo = "Bien",
              Descripcion = bien.Descripcion,
              Fecha = bien.Fecha,
              Estado = bien.StatusId,
              MotivoRechazo = bien.MotivoRechazo,
              Cantidad = bien.Cantidad,
              MontoUnitario = bien.MontoUnitario,
              Monto = bien.Total,
              Enero = bien.Enero == 1,
              Febrero = bien.Febrero == 1,
              Marzo = bien.Marzo == 1,
              Abril = bien.Abril == 1,
              Mayo = bien.Mayo == 1,
              Junio = bien.Junio == 1,
              Julio = bien.Julio == 1,
              Agosto = bien.Agosto == 1,
              Septiembre = bien.Septiembre == 1,
              Octubre = bien.Octubre == 1,
              Noviembre = bien.Noviembre == 1,
              Diciembre = bien.Diciembre == 1
            };
          }
          break;
        case "Gasto":
          var gasto = _context.Gasto
              .Include(g => g.Status)
              .FirstOrDefault(g => g.GastoId == id && g.RoleId == roleId);
          if (gasto != null)
          {
            presupuestoViewModel = new PresupuestoViewModel
            {
              Id = gasto.GastoId,
              Tipo = "Gasto",
              Descripcion = gasto.Justificacion,
              Fecha = gasto.Fecha,
              Estado = gasto.StatusId,
              MotivoRechazo = gasto.MotivoRechazo,
              Justificacion = gasto.Justificacion,
              Monto = gasto.Total,
              Enero = gasto.Enero == 1,
              Febrero = gasto.Febrero == 1,
              Marzo = gasto.Marzo == 1,
              Abril = gasto.Abril == 1,
              Mayo = gasto.Mayo == 1,
              Junio = gasto.Junio == 1,
              Julio = gasto.Julio == 1,
              Agosto = gasto.Agosto == 1,
              Septiembre = gasto.Septiembre == 1,
              Octubre = gasto.Octubre == 1,
              Noviembre = gasto.Noviembre == 1,
              Diciembre = gasto.Diciembre == 1
            };
          }
          break;
        case "Proyecto":
          var proyecto = _context.Proyectos
              .Include(p => p.Status)
              .FirstOrDefault(p => p.ProyectoId == id && p.RoleId == roleId);
          if (proyecto != null)
          {
            presupuestoViewModel = new PresupuestoViewModel
            {
              Id = proyecto.ProyectoId,
              Tipo = "Proyecto",
              Descripcion = proyecto.Descripcion,
              Fecha = proyecto.Fecha,
              Estado = proyecto.StatusId,
              MotivoRechazo = proyecto.MotivoRechazo,
              ValorEstimado = proyecto.ValorEstimado,
              ViabilidadComercial = proyecto.ViabilidadComercial,
              ViabilidadTecnica = proyecto.ViabilidadTecnica,
              ViabilidadLegal = proyecto.ViabilidadLegal,
              ViabilidadGestion = proyecto.ViabilidadGestion,
              ViabilidadImpactoAmbiental = proyecto.ViabilidadImpactoAmbiental,
              ViabilidadFinanciera = proyecto.ViabilidadFinanciera
            };
          }
          break;
        default:
          return NotFound();
      }

      if (presupuestoViewModel == null)
      {
        return NotFound();
      }

      return View(presupuestoViewModel);
    }

    // Acción POST para procesar el formulario de edición
    [HttpPost]
    public IActionResult EditarRechazado(PresupuestoViewModel model)
    {
      // Eliminar el error de validación para MotivoRechazo
      ModelState.Remove("MotivoRechazo");
      ModelState.Remove("RolNombre");

      if (!ModelState.IsValid)
      {
        var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new { x.Key, x.Value.Errors })
            .ToList();

        foreach (var error in errors)
        {
          Console.WriteLine($"Error en el campo '{error.Key}':");
          foreach (var err in error.Errors)
          {
            Console.WriteLine($" - {err.ErrorMessage}");
          }
        }

        return View(model);
      }

      switch (model.Tipo)
      {
        case "Bien":
          var bien = _context.Bienes.Find(model.Id);
          if (bien != null)
          {
            bien.Descripcion = model.Descripcion;
            bien.Cantidad = Convert.ToInt32(model.Cantidad);
            bien.MontoUnitario = model.MontoUnitario;
            bien.Total = model.Cantidad * model.MontoUnitario;
            bien.Enero = model.Enero ? 1 : 0;
            bien.Febrero = model.Febrero ? 1 : 0;
            bien.Marzo = model.Marzo ? 1 : 0;
            bien.Abril = model.Abril ? 1 : 0;
            bien.Mayo = model.Mayo ? 1 : 0;
            bien.Junio = model.Junio ? 1 : 0;
            bien.Julio = model.Julio ? 1 : 0;
            bien.Agosto = model.Agosto ? 1 : 0;
            bien.Septiembre = model.Septiembre ? 1 : 0;
            bien.Octubre = model.Octubre ? 1 : 0;
            bien.Noviembre = model.Noviembre ? 1 : 0;
            bien.Diciembre = model.Diciembre ? 1 : 0;
            _context.Bienes.Update(bien);
            _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_Bienes_Notifications ON Bienes");
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_Bienes_Notifications ON Bienes");
            _context.Database.ExecuteSqlRaw("UPDATE Bienes SET status_ID = status_ID WHERE bien_ID = @p0", model.Id);
          }
          break;
        case "Gasto":
          var gasto = _context.Gasto.Find(model.Id);
          if (gasto != null)
          {
            gasto.Justificacion = model.Justificacion;
            gasto.Total = model.Monto;
            gasto.Enero = model.Enero ? 1 : 0;
            gasto.Febrero = model.Febrero ? 1 : 0;
            gasto.Marzo = model.Marzo ? 1 : 0;
            gasto.Abril = model.Abril ? 1 : 0;
            gasto.Mayo = model.Mayo ? 1 : 0;
            gasto.Junio = model.Junio ? 1 : 0;
            gasto.Julio = model.Julio ? 1 : 0;
            gasto.Agosto = model.Agosto ? 1 : 0;
            gasto.Septiembre = model.Septiembre ? 1 : 0;
            gasto.Octubre = model.Octubre ? 1 : 0;
            gasto.Noviembre = model.Noviembre ? 1 : 0;
            gasto.Diciembre = model.Diciembre ? 1 : 0;
            _context.Gasto.Update(gasto);
            _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_InsertNotification ON Gasto");
            _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_Gasto_Notifications ON Gasto");
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_InsertNotification ON Gasto");
            _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_Gasto_Notifications ON Gasto");
            _context.Database.ExecuteSqlRaw("UPDATE Gasto SET status_ID = status_ID WHERE gasto_ID = @p0", model.Id);
          }
          break;
        case "Proyecto":
          var proyecto = _context.Proyectos.Find(model.Id);
          if (proyecto != null)
          {
            proyecto.Descripcion = model.Descripcion;
            proyecto.ValorEstimado = model.ValorEstimado;
            proyecto.ViabilidadComercial = model.ViabilidadComercial;
            proyecto.ViabilidadTecnica = model.ViabilidadTecnica;
            proyecto.ViabilidadLegal = model.ViabilidadLegal;
            proyecto.ViabilidadGestion = model.ViabilidadGestion;
            proyecto.ViabilidadImpactoAmbiental = model.ViabilidadImpactoAmbiental;
            proyecto.ViabilidadFinanciera = model.ViabilidadFinanciera;
            _context.Proyectos.Update(proyecto);
            _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_Proyectos_Notifications ON Proyectos");
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_Proyectos_Notifications ON Proyectos");
            _context.Database.ExecuteSqlRaw("UPDATE Proyectos SET status_ID = status_ID WHERE proyecto_ID = @p0", model.Id);
          }
          break;
        default:
          return NotFound();
      }

      return RedirectToAction(nameof(Rechazados));
    }

    // Acción para reenviar un presupuesto rechazado
[HttpPost]
public async Task<IActionResult> ReenviarPresupuesto(int id, string tipo)
{
    var roleClaimValue = User.FindFirstValue("RoleID");

    if (string.IsNullOrEmpty(roleClaimValue) || !int.TryParse(roleClaimValue, out var roleId))
    {
        return RedirectToAction("Error", "Home");
    }

    try
    {
        switch (tipo)
        {
            case "Bien":
                var bien = await _context.Bienes.FindAsync(id);
                if (bien != null)
                {
                    bien.StatusId = 2; // 2 = Pendiente
                    _context.Bienes.Update(bien);
                    _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_Bienes_Notifications ON Bienes");
                    await _context.SaveChangesAsync();
                    _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_Bienes_Notifications ON Bienes");
                    _context.Database.ExecuteSqlRaw("UPDATE Bienes SET status_ID = status_ID WHERE bien_ID = @p0", id);
                }
                break;

            case "Gasto":
                var gasto = await _context.Gasto.FindAsync(id);
                if (gasto != null)
                {
                    gasto.StatusId = 2; // 2 = Pendiente
                    _context.Gasto.Update(gasto);
                    _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_InsertNotification ON Gasto");
                    _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_Gasto_Notifications ON Gasto");
              await _context.SaveChangesAsync();
                    _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_InsertNotification ON Gasto");
                    _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_Gasto_Notifications ON Gasto");

              _context.Database.ExecuteSqlRaw("UPDATE Gasto SET status_ID = status_ID WHERE gasto_ID = @p0", id);

                }
                break;

            case "Proyecto":
                var proyecto = await _context.Proyectos.FindAsync(id);
                if (proyecto != null)
                {
                    proyecto.StatusId = 2; // 2 = Pendiente
                    _context.Proyectos.Update(proyecto);
                    _context.Database.ExecuteSqlRaw("DISABLE TRIGGER trg_Proyectos_Notifications ON Proyectos");
                    await _context.SaveChangesAsync();
                    _context.Database.ExecuteSqlRaw("ENABLE TRIGGER trg_Proyectos_Notifications ON Proyectos");
                    _context.Database.ExecuteSqlRaw("UPDATE Proyectos SET status_ID = status_ID WHERE proyecto_ID = @p0", id);
                }
                break;

            default:
                return Json(new { success = false, message = "Tipo de presupuesto no válido." });
        }

        return Json(new { success = true, message = "Presupuesto reenviado correctamente." });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = "Ocurrió un error al reenviar el presupuesto." });
    }
}


    //[HttpPost]
    //public IActionResult Aprobar(int id, string tipo)
    //{
    //  var roleClaimValue = User.FindFirstValue("RoleID");

    //  if (string.IsNullOrEmpty(roleClaimValue) || !int.TryParse(roleClaimValue, out var roleId))
    //  {
    //    return RedirectToAction("Error", "Home");
    //  }

    //  switch (tipo)
    //  {
    //    case "Bien":
    //      var bien = _context.Bienes.FirstOrDefault(b => b.BienId == id && b.RoleId == roleId);
    //      if (bien != null)
    //      {
    //        bien.StatusId = 3; // 3 = Aprobado
    //        _context.Bienes.Update(bien);
    //        _context.SaveChanges();
    //      }
    //      break;
    //    case "Gasto":
    //      var gasto = _context.Gasto.FirstOrDefault(g => g.GastoId == id && g.RoleId == roleId);
    //      if (gasto != null)
    //      {
    //        gasto.StatusId = 3; // 3 = Aprobado
    //        _context.Gasto.Update(gasto);
    //        _context.SaveChanges();
    //      }
    //      break;
    //    case "Proyecto":
    //      var proyecto = _context.Proyectos.FirstOrDefault(p => p.ProyectoId == id && p.RoleId == roleId);
    //      if (proyecto != null)
    //      {
    //        proyecto.StatusId = 3; // 3 = Aprobado
    //        _context.Proyectos.Update(proyecto);
    //        _context.SaveChanges();
    //      }
    //      break;
    //    default:
    //      return NotFound();
    //  }

    //  return RedirectToAction(nameof(Index));
    //}

    //[HttpPost]
    //public IActionResult Rechazar(int id, string tipo, string motivoRechazo)
    //{
    //  var roleClaimValue = User.FindFirstValue("RoleID");

    //  if (string.IsNullOrEmpty(roleClaimValue) || !int.TryParse(roleClaimValue, out var roleId))
    //  {
    //    return RedirectToAction("Error", "Home");
    //  }

    //  switch (tipo)
    //  {
    //    case "Bien":
    //      var bien = _context.Bienes.FirstOrDefault(b => b.BienId == id && b.RoleId == roleId);
    //      if (bien != null)
    //      {
    //        bien.StatusId = 1; // 1 = Rechazado
    //        bien.MotivoRechazo = motivoRechazo;
    //        _context.Bienes.Update(bien);
    //        _context.SaveChanges();
    //      }
    //      break;
    //    case "Gasto":
    //      var gasto = _context.Gasto.FirstOrDefault(g => g.GastoId == id && g.RoleId == roleId);
    //      if (gasto != null)
    //      {
    //        gasto.StatusId = 1; // 1 = Rechazado
    //        gasto.MotivoRechazo = motivoRechazo;
    //        _context.Gasto.Update(gasto);
    //        _context.SaveChanges();
    //      }
    //      break;
    //    case "Proyecto":
    //      var proyecto = _context.Proyectos.FirstOrDefault(p => p.ProyectoId == id && p.RoleId == roleId);
    //      if (proyecto != null)
    //      {
    //        proyecto.StatusId = 1; // 1 = Rechazado
    //        proyecto.MotivoRechazo = motivoRechazo;
    //        _context.Proyectos.Update(proyecto);
    //        _context.SaveChanges();
    //      }
    //      break;
    //    default:
    //      return NotFound();
    //  }

    //  return RedirectToAction("Index", "VerPresupuesto"); // Redirige al Index de VerPresupuesto
    //}

  }
}
