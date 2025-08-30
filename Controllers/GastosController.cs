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
      // Obtener el RoleID del usuario logueado desde los claims
      var roleIdClaim = User.Claims.FirstOrDefault(c => c.Type == "RoleID");
      if (roleIdClaim == null || !int.TryParse(roleIdClaim.Value, out int roleId))
      {
        return Unauthorized();
      }

      // Base query: solo gastos aprobados (StatusId == 3)
      var query = _context.Gasto
          .Include(g => g.Role)
          .Include(g => g.Status)
          .Where(g => g.StatusId == 3);

      // Si el rol NO es 2, filtrar por su propio rol
      if (roleId != 2)
      {
        query = query.Where(g => g.RoleId == roleId);
      }

      // Agrupar y preparar el modelo
      var gastosPorCategoria = query
          .GroupBy(g => new { g.CuentaMadreId, g.CuentaHijaId })
          .Select(g => new
          {
            Categoria = g.Key.CuentaMadreId,
            CuentaHijaId = g.Key.CuentaHijaId,
            Gastos = g.ToList()
          })
          .ToList()
          .Select(g => new GastoPorCategoriaViewModel
          {
            Categoria = g.Categoria,
            NombreCategoria = ObtenerNombreCategoria(g.Categoria),
            CuentaHijaId = g.CuentaHijaId,
            NombreCuentaHija = ObtenerNombreCuentaHija(g.CuentaHijaId),
            Gastos = g.Gastos
          })
          .ToList();

      return View("~/Views/ResumenGastos/Index.cshtml", gastosPorCategoria);
    }


    private static string ObtenerNombreCategoria(int cuentaMadreId)
    {
      // Definir las categorías (Cuentas Madre)
      var categorias = new Dictionary<int, string>
    {
        { 441, "Gastos del Personal" },
        { 442, "Gastos Servicios Externos" },
        { 443, "Gastos Movilidad y Comunicación" },
        { 444, "Gastos Infraestructura" },
        { 445, "Gastos Generales" }
    };

      // Obtener el nombre de la categoría
      return categorias.ContainsKey(cuentaMadreId) ? categorias[cuentaMadreId] : "Sin nombre";
    }

    // Método para obtener el nombre de la CuentaHija desde el código
    private static string ObtenerNombreCuentaHija(int cuentaHijaId)
    {
      // Definir las cuentas madre e hijas
      var cuentas = new Dictionary<int, string>
    {
        { 44101000, "Sueldos y Bonificaciones" },
        { 44105000, "Viáticos" },
        { 44106000, "Décimo tercer sueldo" },
        { 44108000, "Incentivos" },
        { 44112000, "Cargas Sociales Patronales" },
        { 44113000, "Refrigerios" },
        { 44114000, "Vestimenta" },
        { 44115000, "Capacitación" },
        { 44116000, "Seguros para personal" },
        { 44199000, "Otros gastos de personal" },
        { 44201000, "Servicios de Computación" },
        { 44202000, "Servicios de Seguridad" },
        { 44203000, "Servicios de Información" },
        { 44204000, "Servicios de Limpieza" },
        { 44205000, "Asesoría Jurídica" },
        { 44206000, "Auditoría Externa" },
        { 44207000, "Consultoría Externa" },
        { 44208000, "Servicios Médicos" },
        { 44210000, "Servicios de Mensajería" },
        { 44212000, "Servicios de Gestión de Riesgos" },
        { 44299000, "Otros Servicios Contratados" },
        { 44301000, "Pasajes y fletes" },
        { 44304000, "Alquiler de Vehículos" },
        { 44307000, "Teléfono y Fax" },
        { 44399000, "Otros gastos de movilidad" },
        { 44401000, "Seguros sobre bienes de uso" },
        { 44403000, "Mantenimiento de inmuebles, mobiliario y equipo" },
        { 44404000, "Agua y Energía Eléctrica" },
        { 44407000, "Depreciación de inmuebles, mobiliario y equipo" },
        { 44499000, "Otros gastos de infraestructura" },
        { 44503000, "Otros seguros" },
        { 44505000, "Amortización de Otros Cargos Diferidos" },
        { 44506000, "Papelería, Útiles y otros materiales" },
        { 44507000, "Gastos Legales" },
        { 44508000, "Suscripciones y afiliaciones" },
        { 44509000, "Promoción y Publicidad" },
        { 44512000, "Amortización de Software" },
        { 44515000, "Gastos por materiales y suministros" },
        { 44517000, "Aporte al presupuesto de la SUGEF" },
        { 44599000, "Gastos Generales Diversos" }
    };

      // Obtener el nombre de la CuentaHija
      return cuentas.ContainsKey(cuentaHijaId) ? cuentas[cuentaHijaId] : "Sin nombre";
    }
  }
}
