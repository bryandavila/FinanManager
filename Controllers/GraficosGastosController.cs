using Microsoft.AspNetCore.Mvc;
using FinanManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims; // Necesario para acceder a los Claims
using Microsoft.AspNetCore.Authorization; // Opcional: para asegurar que el usuario esté logueado

namespace FinanManager.Controllers
{
  [Authorize] // Buena práctica: Asegura que solo usuarios autenticados puedan acceder
  public class GraficosGastosController : Controller
  {
    private readonly FinanManagerContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GraficosGastosController(FinanManagerContext context, IHttpContextAccessor httpContextAccessor)
    {
      _context = context;
      _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Index()
    {
      // Obtener el RoleId del usuario logueado desde el Claim personalizado "RoleID"
      var roleIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("RoleID")?.Value;
      var roleId = !string.IsNullOrEmpty(roleIdClaim) ? int.Parse(roleIdClaim) : 0;

      // Si por alguna razón no se encuentra el rol, no mostrar datos
      if (roleId == 0)
      {
        ViewBag.DatosGraficos = new List<object>(); // Enviar lista vacía a la vista
        return View();
      }

      // Iniciar la consulta base de gastos
      var query = _context.Gasto.AsQueryable();

      // 1. Filtrar siempre por StatusId = 3 (Aprobado)
      query = query.Where(g => g.StatusId == 3);

      // 2. Si el rol no es 2 (admin de gastos), filtrar además por el RoleId del usuario
      if (roleId != 2)
      {
        query = query.Where(g => g.RoleId == roleId);
      }

      // Realizar la agrupación y suma sobre la consulta ya filtrada
      var gastosPorCategoria = query
          .GroupBy(g => g.CuentaMadreId)
          .Select(g => new
          {
            Categoria = g.Key,
            Total = g.Sum(x => x.Total)
          })
          .ToList();

      var datosGraficos = gastosPorCategoria.Select(g => new
      {
        Categoria = ObtenerNombreCategoria(g.Categoria),
        Total = g.Total
      }).ToList();

      ViewBag.DatosGraficos = datosGraficos;

      return View();
    }

    private string ObtenerNombreCategoria(int cuentaMadreId)
    {
      var categorias = new Dictionary<int, string>
            {
                { 441, "Gastos del Personal" },
                { 442, "Gastos Servicios Externos" },
                { 443, "Gastos Movilidad y Comunicación" },
                { 444, "Gastos Infraestructura" },
                { 445, "Gastos Generales" }
            };

      return categorias.ContainsKey(cuentaMadreId) ? categorias[cuentaMadreId] : "Sin nombre";
    }
  }
}
