using FinanManager.Models;
using FinanManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using Document = QuestPDF.Fluent.Document;

namespace FinanManager.Controllers
{
  [AuthorizeRole(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21)]
  public class SeguimientoGastosController : Controller
  {
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;
    private readonly FinanManagerContext _context;

    public SeguimientoGastosController(IConfiguration configuration, IEmailSender emailSender, FinanManagerContext context)
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
    public IActionResult CompararMeses(List<int> meses, int anio)
    {
      if (meses == null || meses.Count < 2 || anio == 0)
      {
        return BadRequest("Debe seleccionar al menos dos meses y un año para comparar.");
      }

      var roleIdClaim = User.Claims.FirstOrDefault(c => c.Type == "RoleID");
      if (roleIdClaim == null || !int.TryParse(roleIdClaim.Value, out int roleId))
      {
        return Unauthorized();
      }

      var mesesUnicos = meses.Distinct().ToList();
      var gastosPorMes = new Dictionary<string, decimal>();

      foreach (var mes in mesesUnicos)
      {
        var nombreMes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mes);

        var query = _context.Gasto.Where(g =>
          g.StatusId == 3 &&
          g.Fecha.Month == mes &&
          g.Fecha.Year == anio
        );

        if (roleId != 2)
        {
          query = query.Where(g => g.RoleId == roleId);
        }

        decimal totalGastos = query.Sum(g => g.Total);
        gastosPorMes.Add(nombreMes, totalGastos);
      }

      return Json(gastosPorMes);
    }

    [HttpGet]
    public IActionResult GastoPorMes(int mes = 0)
    {
      var roleIdClaim = User.Claims.FirstOrDefault(c => c.Type == "RoleID");
      if (roleIdClaim == null || !int.TryParse(roleIdClaim.Value, out int roleId))
      {
        return Unauthorized();
      }

      var query = _context.Gasto.Where(g => g.StatusId == 3);

      if (mes != 0)
      {
        query = query.Where(g => g.Fecha.Month == mes);
      }

      if (roleId != 2)
      {
        query = query.Where(g => g.RoleId == roleId);
      }

      var gastos = query.ToList();

      return View("GastoPorMes", gastos);
    }



    [HttpPost]
    public IActionResult ExportarReportePDF([FromBody] ExportRequest request)
    {
      var roleIdClaim = User.Claims.FirstOrDefault(c => c.Type == "RoleID");
      if (roleIdClaim == null || !int.TryParse(roleIdClaim.Value, out int roleId))
      {
        return Unauthorized();
      }

      var data = _context.Gasto
          .Where(g => request.Categories.Contains(g.CuentaMadreId.ToString()))
          .Where(g => g.Fecha >= request.StartDate && g.Fecha <= request.EndDate)
          .Where(g => g.StatusId == 3);

      if (roleId != 2)
      {
        data = data.Where(g => g.RoleId == roleId);
      }

      var listaFinal = data.ToList();

      var pdf = Document.Create(container =>
      {
        container.Page(page =>
        {
          page.Size(PageSizes.A4);
          page.Margin(2, Unit.Centimetre);

          page.Header()
              .Text("Reporte de Gastos")
              .FontSize(20)
              .Bold()
              .AlignCenter();

          page.Content()
              .PaddingVertical(1, Unit.Centimetre)
              .Table(table =>
              {
                table.ColumnsDefinition(columns =>
                {
                  columns.RelativeColumn();
                  columns.RelativeColumn();
                  columns.RelativeColumn();
                  columns.RelativeColumn();
                  columns.RelativeColumn();
                  columns.RelativeColumn();
                  columns.RelativeColumn();
                });

                table.Header(header =>
                {
                  header.Cell().Text("Gasto ID").FontSize(12).Bold();
                  header.Cell().Text("Cuenta Madre ID").FontSize(12).Bold();
                  header.Cell().Text("Cuenta Hija ID").FontSize(12).Bold();
                  header.Cell().Text("Justificación").FontSize(12).Bold();
                  header.Cell().Text("Total").FontSize(12).Bold();
                  header.Cell().Text("Fecha").FontSize(12).Bold();
                  header.Cell().Text("Motivo Rechazo").FontSize(12).Bold();
                });

                foreach (var gasto in listaFinal)
                {
                  table.Cell().Text(gasto.GastoId.ToString()).FontSize(10);
                  table.Cell().Text(gasto.CuentaMadreId.ToString()).FontSize(10);
                  table.Cell().Text(gasto.CuentaHijaId.ToString()).FontSize(10);
                  table.Cell().Text(gasto.Justificacion).FontSize(10);
                  table.Cell().Text(gasto.Total.ToString("0.00")).FontSize(10);
                  table.Cell().Text(gasto.Fecha.ToShortDateString()).FontSize(10);
                  table.Cell().Text(gasto.MotivoRechazo ?? "N/A").FontSize(10);
                }
              });

          page.Footer()
              .AlignCenter()
              .Text(text =>
              {
                text.Span("Página ");
                text.CurrentPageNumber();
              });
        });
      });

      var pdfBytes = pdf.GeneratePdf();
      return File(pdfBytes, "application/pdf", "ReporteGastos.pdf");
    }


    private List<Gasto> ObtenerDatosParaExportar(ExportRequest request)
    {
      return _context.Gasto
          .Where(g => request.Categories.Contains(g.CuentaMadreId.ToString()))
          .Where(g => g.Fecha >= request.StartDate && g.Fecha <= request.EndDate)
          .ToList();
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
  

  [HttpPost]
    public IActionResult EliminarAlerta(int id)
    {
      // Buscar la alerta por ID
      var alerta = _context.AlertConfigs.FirstOrDefault(a => a.Id == id);

      if (alerta == null)
      {
        return NotFound();
      }

      // Verificar que el usuario tenga permiso para eliminar esta alerta
      var roleIdClaim = User.Claims.FirstOrDefault(c => c.Type == "RoleID");
      if (roleIdClaim == null || int.Parse(roleIdClaim.Value) != alerta.RoleId)
      {
        return Forbid(); // O Unauthorized() dependiendo de tus necesidades
      }

      // Eliminar la alerta
      _context.AlertConfigs.Remove(alerta);
      _context.SaveChanges();

      return RedirectToAction("ConfigurarAlertas");
    }
  }
}
