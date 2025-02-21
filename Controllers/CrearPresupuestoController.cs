using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using FinanManager.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FinanManager.Controllers
{
  public class CrearPresupuestoController : Controller
  {
    private readonly FinanManagerContext _context;

    public CrearPresupuestoController(FinanManagerContext context)
    {
      _context = context;
    }

    public IActionResult Index()
    {
      return View();
    }

    private int GetCurrentUserRoleId()
    {
      var claimsIdentity = User.Identity as ClaimsIdentity;
      var roleClaim = claimsIdentity?.FindFirst("RoleID");
      return roleClaim != null ? int.Parse(roleClaim.Value) : 0;
    }

    //Crear Bienes
    [HttpPost]
    public async Task<IActionResult> CrearBien([FromBody] Bienes bien)
    {
      if (bien == null)
      {
        return Json(new { success = false, message = "Datos inválidos." });
      }

      // Asignar valores adicionales
      bien.RoleId = GetCurrentUserRoleId();
      bien.StatusId = 2; // Pendiente por defecto

      // Ignorar la validación de las propiedades "Role" y "MotivoRechazo"
      ModelState.Remove("Role");
      ModelState.Remove("MotivoRechazo");

      // Validar el modelo después de asignar valores faltantes
      if (!TryValidateModel(bien))
      {
        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
        return Json(new { success = false, message = "Error al validar el bien.", errors });
      }

      try
      {
        // Llamar al procedimiento almacenado
        var bienId = await _context.Database.ExecuteSqlRawAsync(
            "EXEC CrearBien @descripcion, @cantidad, @montoUnitario, @total, @enero, @febrero, @marzo, @abril, @mayo, @junio, @julio, @agosto, @septiembre, @octubre, @noviembre, @diciembre, @role_ID, @status, @MotivoRechazo",
            new SqlParameter("@descripcion", bien.Descripcion),
            new SqlParameter("@cantidad", bien.Cantidad),
            new SqlParameter("@montoUnitario", bien.MontoUnitario),
            new SqlParameter("@total", bien.Total),
            new SqlParameter("@enero", bien.Enero),
            new SqlParameter("@febrero", bien.Febrero),
            new SqlParameter("@marzo", bien.Marzo),
            new SqlParameter("@abril", bien.Abril),
            new SqlParameter("@mayo", bien.Mayo),
            new SqlParameter("@junio", bien.Junio),
            new SqlParameter("@julio", bien.Julio),
            new SqlParameter("@agosto", bien.Agosto),
            new SqlParameter("@septiembre", bien.Septiembre),
            new SqlParameter("@octubre", bien.Octubre),
            new SqlParameter("@noviembre", bien.Noviembre),
            new SqlParameter("@diciembre", bien.Diciembre),
            new SqlParameter("@role_ID", bien.RoleId),
            new SqlParameter("@status", bien.StatusId),
            new SqlParameter("@MotivoRechazo", bien.MotivoRechazo ?? (object)DBNull.Value)
        );

        return Json(new { success = true, message = "Bien creado exitosamente.", bienId });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Ocurrió un error interno al procesar la solicitud.", error = ex.Message });
      }
    }

    //Crear Gatos
    [HttpPost]
    public async Task<IActionResult> CrearGasto([FromBody] Gasto gasto)
    {
      if (gasto == null)
      {
        return Json(new { success = false, message = "Datos inválidos." });
      }

      try
      {
        // Asignar valores adicionales
        gasto.RoleId = GetCurrentUserRoleId();
        gasto.StatusId = 2; // Pendiente

        // Ignorar la validación de las propiedades de navegación
        ModelState.Remove("Role");
        ModelState.Remove("Status");
        ModelState.Remove("MotivoRechazo");

        // Validar el modelo después de asignar valores faltantes
        if (!TryValidateModel(gasto))
        {
          var errors = ModelState.Values
              .SelectMany(v => v.Errors)
              .Select(e => e.ErrorMessage)
              .ToList();
          return Json(new { success = false, message = "Error al validar el gasto.", errors });
        }

        // Llamar al procedimiento almacenado
        var gastoId = await _context.Database.ExecuteSqlRawAsync(
            "EXEC CrearGasto @cuentaMadre_ID, @cuentaHija_ID, @justificacion, @total, @enero, @febrero, @marzo, @abril, @mayo, @junio, @julio, @agosto, @septiembre, @octubre, @noviembre, @diciembre, @role_ID, @status_ID, @MotivoRechazo",
            new SqlParameter("@cuentaMadre_ID", gasto.CuentaMadreId),
            new SqlParameter("@cuentaHija_ID", gasto.CuentaHijaId),
            new SqlParameter("@justificacion", gasto.Justificacion),
            new SqlParameter("@total", gasto.Total),
            new SqlParameter("@enero", gasto.Enero),
            new SqlParameter("@febrero", gasto.Febrero),
            new SqlParameter("@marzo", gasto.Marzo),
            new SqlParameter("@abril", gasto.Abril),
            new SqlParameter("@mayo", gasto.Mayo),
            new SqlParameter("@junio", gasto.Junio),
            new SqlParameter("@julio", gasto.Julio),
            new SqlParameter("@agosto", gasto.Agosto),
            new SqlParameter("@septiembre", gasto.Septiembre),
            new SqlParameter("@octubre", gasto.Octubre),
            new SqlParameter("@noviembre", gasto.Noviembre),
            new SqlParameter("@diciembre", gasto.Diciembre),
            new SqlParameter("@role_ID", gasto.RoleId),
            new SqlParameter("@status_ID", gasto.StatusId),
            new SqlParameter("@MotivoRechazo", gasto.MotivoRechazo ?? (object)DBNull.Value)
        );

        return Json(new { success = true, message = "Gasto registrado correctamente.", gastoId });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Ocurrió un error interno al procesar la solicitud.", error = ex.Message });
      }
    }


    //Crear proyecto
    [HttpPost]
    public async Task<IActionResult> CrearProyecto([FromBody] Proyecto proyecto)
    {
      if (proyecto == null)
      {
        return Json(new { success = false, message = "Datos inválidos." });
      }

      // Verificar que la descripción no sea nula o vacía
      if (string.IsNullOrEmpty(proyecto.Descripcion))
      {
        return Json(new { success = false, message = "La descripción del proyecto es requerida." });
      }

      try
      {
        // Asignar valores adicionales
        proyecto.RoleId = GetCurrentUserRoleId();
        proyecto.StatusId = 2; // Pendiente

        // Ignorar la validación de las propiedades de navegación
        ModelState.Remove("Role");
        ModelState.Remove("StatusId"); // Ignorar la validación de StatusId
        ModelState.Remove("MotivoRechazo");

        // Validar el modelo después de asignar valores faltantes
        if (!TryValidateModel(proyecto))
        {
          var errors = ModelState.Values
              .SelectMany(v => v.Errors)
              .Select(e => e.ErrorMessage)
              .ToList();
          return Json(new { success = false, message = "Error al validar el proyecto.", errors });
        }

        // Llamar al procedimiento almacenado
        var proyectoId = await _context.Database.ExecuteSqlRawAsync(
            "EXEC CrearProyecto @valorEstimado, @descripcion, @viabilidadComercial, @viabilidadTecnica, @viabilidadLegal, @viabilidadGestion, @viabilidadImpactoAmbiental, @viabilidadFinanciera, @role_ID, @status_ID, @MotivoRechazo",
            new SqlParameter("@valorEstimado", proyecto.ValorEstimado),
            new SqlParameter("@descripcion", proyecto.Descripcion ?? (object)DBNull.Value),
            new SqlParameter("@viabilidadComercial", proyecto.ViabilidadComercial ?? (object)DBNull.Value),
            new SqlParameter("@viabilidadTecnica", proyecto.ViabilidadTecnica ?? (object)DBNull.Value),
            new SqlParameter("@viabilidadLegal", proyecto.ViabilidadLegal ?? (object)DBNull.Value),
            new SqlParameter("@viabilidadGestion", proyecto.ViabilidadGestion ?? (object)DBNull.Value),
            new SqlParameter("@viabilidadImpactoAmbiental", proyecto.ViabilidadImpactoAmbiental ?? (object)DBNull.Value),
            new SqlParameter("@viabilidadFinanciera", proyecto.ViabilidadFinanciera ?? (object)DBNull.Value),
            new SqlParameter("@role_ID", proyecto.RoleId),
            new SqlParameter("@status_ID", proyecto.StatusId),
            new SqlParameter("@MotivoRechazo", proyecto.MotivoRechazo ?? (object)DBNull.Value)
        );


        return Json(new { success = true, message = "Proyecto registrado correctamente.", proyectoId });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Ocurrió un error interno al procesar la solicitud.", error = ex.Message });
      }
    }
  }
}
