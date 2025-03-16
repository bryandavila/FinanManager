using FinanManager.Models;
using FinanManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace FinanManager.Controllers;
public class SeguimientoGastosController : Controller
{
  private readonly IConfiguration _configuration;
  private readonly IEmailSender _emailSender;
  private readonly FinanManagerContext _context;

  public SeguimientoGastosController (IConfiguration configuration, IEmailSender emailSender, FinanManagerContext context)
  {
    _configuration = configuration;
    _emailSender = emailSender;
    _context = context;
  }
  public IActionResult Index()
    {
      return View();
    }

  [HttpGet]
  public IActionResult ConfigurarAlertas()
  {
    // Obtener el RoleID del usuario autenticado
    var roleIdClaim = User.Claims.FirstOrDefault(c => c.Type == "RoleID");
    if (roleIdClaim == null)
    {
      // Manejar el caso en que el claim no esté presente
      return RedirectToAction("Error", "Home");
    }

    int roleId = int.Parse(roleIdClaim.Value);

    // Pasar el RoleID a la vista
    ViewBag.RoleID = roleId;

    var alertConfigs = _context.AlertConfigs
            .Where(a => a.RoleId == roleId)
            .ToList();

    return View(alertConfigs);
  }

  [HttpPost]
  public IActionResult ConfigurarAlertas(AlertConfig model)
  {
    // Aquí puedes procesar la configuración de alertas
    // El model.RoleId ya contiene el RoleID del usuario autenticado

    // Ejemplo: Guardar la configuración en la base de datos
    using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("FinanManagerDBConnection")))
    {
      conn.Open();
      SqlCommand cmd = new SqlCommand("sp_GuardarConfiguracionAlertas", conn)
      {
        CommandType = CommandType.StoredProcedure
      };
      cmd.Parameters.AddWithValue("@RoleID", model.RoleId);
      cmd.Parameters.AddWithValue("@SpendingLimit", model.SpendingLimit);
      cmd.Parameters.AddWithValue("@NearLimitValue", model.NearLimitValue);
      cmd.Parameters.AddWithValue("@NearLimitAlert", model.NearLimitAlert);
      cmd.Parameters.AddWithValue("@ExceedLimitAlert", model.ExceedLimitAlert);


      cmd.ExecuteNonQuery();
    }

    return RedirectToAction("Index", "SeguimientoGastos"); // Redirigir a la página principal
  }
}


