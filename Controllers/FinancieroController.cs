using FinanManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FinanManager.Controllers
{
  [AuthorizeRole(2)]
  public class FinancieroController : Controller
  {
    private readonly string _connectionString;
    private readonly FinanManagerContext _context;

    public FinancieroController(IConfiguration configuration, FinanManagerContext context)
    {
      _connectionString = configuration.GetConnectionString("FinanManagerDBConnection");
      _context = context;
    }

    public IActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> DescargarReporteAnual(int año, string formato)
    {
      if (año < 1900 || año > DateTime.Now.Year + 1)
      {
        return BadRequest("El año ingresado no es válido.");
      }

      var gastos = await EjecutarProcedimiento("ObtenerReporteAnual", new SqlParameter("@Año", año));

      if (gastos.Rows.Count == 0)
      {
        return NoContent(); // Devuelve 204 No Content
      }

      return formato.ToLower() switch
      {
        "excel" => GenerarExcel(gastos, $"ReporteAnual_{año}.xlsx"),
        "pdf" => GenerarPdf(gastos, $"ReporteAnual_{año}.pdf"),
        _ => BadRequest("Formato no soportado. Use 'excel' o 'pdf'.")
      };
    }

    [HttpGet]
    public async Task<IActionResult> DescargarReporteMensual(int año, int mes, string formato)
    {
      if (año < 1900 || año > DateTime.Now.Year + 1 || mes < 1 || mes > 12)
      {
        return BadRequest("El año o mes ingresado no es válido.");
      }

      var gastos = await EjecutarProcedimiento("ObtenerReporteMensual",
          new SqlParameter("@Año", año),
          new SqlParameter("@Mes", mes));

      if (gastos.Rows.Count == 0)
      {
        return NoContent(); // Devuelve 204 No Content
      }

      return formato.ToLower() switch
      {
        "excel" => GenerarExcel(gastos, $"ReporteMensual_{año}_{mes}.xlsx"),
        "pdf" => GenerarPdf(gastos, $"ReporteMensual_{año}_{mes}.pdf"),
        _ => BadRequest("Formato no soportado. Use 'excel' o 'pdf'.")
      };
    }

    [HttpGet]
    public async Task<IActionResult> DescargarReporteDiario(DateTime fecha, string formato)
    {
      if (fecha == default)
      {
        return BadRequest("La fecha ingresada no es válida.");
      }

      var gastos = await EjecutarProcedimiento("ObtenerReporteDiario", new SqlParameter("@Fecha", fecha.Date));

      if (gastos.Rows.Count == 0)
      {
        return NoContent(); // Devuelve 204 No Content
      }

      return formato.ToLower() switch
      {
        "excel" => GenerarExcel(gastos, $"ReporteDiario_{fecha:yyyy_MM_dd}.xlsx"),
        "pdf" => GenerarPdf(gastos, $"ReporteDiario_{fecha:yyyy_MM_dd}.pdf"),
        _ => BadRequest("Formato no soportado. Use 'excel' o 'pdf'.")
      };
    }

    private async Task<DataTable> EjecutarProcedimiento(string procedimiento, params SqlParameter[] parameters)
    {
      using (var connection = new SqlConnection(_connectionString))
      {
        await connection.OpenAsync();
        using (var command = new SqlCommand(procedimiento, connection))
        {
          command.CommandType = CommandType.StoredProcedure;
          if (parameters != null)
          {
            command.Parameters.AddRange(parameters);
          }
          using (var reader = await command.ExecuteReaderAsync())
          {
            var table = new DataTable();
            table.Load(reader);
            return table;
          }
        }
      }
    }

    private IActionResult GenerarExcel(DataTable gastos, string nombreArchivo)
    {
      using (var workbook = new XLWorkbook())
      {
        var worksheet = workbook.Worksheets.Add("Gastos");
        worksheet.Cell(1, 1).Value = "ID";
        worksheet.Cell(1, 2).Value = "Justificación";
        worksheet.Cell(1, 3).Value = "Total";
        worksheet.Cell(1, 4).Value = "Fecha";
        worksheet.Cell(1, 5).Value = "Rol";

        int row = 2;
        decimal totalGastos = 0;

        foreach (DataRow gasto in gastos.Rows)
        {
          worksheet.Cell(row, 1).Value = Convert.ToInt32(gasto["GastoId"]);
          worksheet.Cell(row, 2).Value = gasto["Justificacion"].ToString();
          worksheet.Cell(row, 3).Value = Convert.ToDecimal(gasto["Total"]);
          worksheet.Cell(row, 4).Value = Convert.ToDateTime(gasto["Fecha"]).ToString("yyyy-MM-dd");
          worksheet.Cell(row, 5).Value = gasto["RolNombre"].ToString();
          totalGastos += Convert.ToDecimal(gasto["Total"]);
          row++;
        }

        worksheet.Cell(row, 2).Value = "Total Gastos:";
        worksheet.Cell(row, 3).Value = totalGastos;
        worksheet.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        using (var stream = new MemoryStream())
        {
          workbook.SaveAs(stream);
          var content = stream.ToArray();
          return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }
      }
    }

    private IActionResult GenerarPdf(DataTable gastos, string nombreArchivo)
    {
      using (var memoryStream = new MemoryStream())
      {
        iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 25, 25, 30, 30);
        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        document.Open();

        document.Add(new Paragraph("Reporte de Gastos") { Alignment = Element.ALIGN_CENTER });
        document.Add(new Paragraph($"Total de registros: {gastos.Rows.Count}") { SpacingAfter = 20 });

        PdfPTable table = new PdfPTable(5) { WidthPercentage = 100 };
        table.SetWidths(new float[] { 1f, 3f, 1.5f, 1.5f, 2f });

        table.AddCell(new PdfPCell(new Phrase("ID")) { BackgroundColor = BaseColor.LIGHT_GRAY });
        table.AddCell(new PdfPCell(new Phrase("Justificación")) { BackgroundColor = BaseColor.LIGHT_GRAY });
        table.AddCell(new PdfPCell(new Phrase("Total")) { BackgroundColor = BaseColor.LIGHT_GRAY });
        table.AddCell(new PdfPCell(new Phrase("Fecha")) { BackgroundColor = BaseColor.LIGHT_GRAY });
        table.AddCell(new PdfPCell(new Phrase("Rol")) { BackgroundColor = BaseColor.LIGHT_GRAY });

        decimal totalGastos = 0;

        foreach (DataRow gasto in gastos.Rows)
        {
          table.AddCell(gasto["GastoId"].ToString());
          table.AddCell(gasto["Justificacion"].ToString());
          table.AddCell(Convert.ToDecimal(gasto["Total"]).ToString("C"));
          table.AddCell(Convert.ToDateTime(gasto["Fecha"]).ToString("yyyy-MM-dd"));
          table.AddCell(gasto["RolNombre"].ToString());
          totalGastos += Convert.ToDecimal(gasto["Total"]);
        }

        document.Add(table);
        document.Add(new Paragraph($"Total de Gastos: {totalGastos:C}") { SpacingBefore = 20 });

        document.Close();
        writer.Close();

        return File(memoryStream.ToArray(), "application/pdf", nombreArchivo);
      }
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerGastosGenerales()
    {
      try
      {
        // Llamada al procedimiento almacenado para obtener los gastos generales
        var gastos = await EjecutarProcedimiento("ObtenerGastosGenerales");

        // Verifica si no hay datos
        if (gastos.Rows.Count == 0)
        {
          return Ok(new { message = "No hay datos disponibles", data = new List<object>() });
        }

        // Ajustamos para seleccionar las columnas correctas de la tabla
        var listaGastos = gastos.AsEnumerable().Select(row => new
        {
          Rol = row["Rol"].ToString(),  // Cambié Categoria por Rol, que es el campo de tu procedimiento
          TotalGastos = Convert.ToDecimal(row["TotalGastos"])
        }).ToList();

        return Ok(new { data = listaGastos });
      }
      catch (Exception ex)
      {
        // Manejo de errores
        return BadRequest(new { message = "Error al obtener datos", error = ex.Message });
      }
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerGastosPorRol(int rolId)
    {
      try
      {
        // Llamada al procedimiento almacenado para obtener los gastos por rol
        var gastos = await EjecutarProcedimiento("ObtenerGastosPorRol", new SqlParameter("@RolId", rolId));

        // Convertir DataTable a una lista de objetos con los datos correctos
        var resultado = gastos.AsEnumerable().Select(row => new
        {
          CuentaHija = row.Field<int>("CuentaHija"),  // Confirmamos que sea 'CuentaHija'
          TotalGastos = row.Field<decimal>("TotalGastos")  // Confirmamos que sea 'TotalGastos'
        }).ToList();

        return Ok(resultado); // Devuelve un JSON válido
      }
      catch (Exception ex)
      {
        // Manejo de errores
        return StatusCode(500, new { error = $"Error interno del servidor: {ex.Message}" });
      }
    }

    //Comparativos anuales
    [HttpGet]
    public async Task<IActionResult> ObtenerComparativoAnualAprobados(int año1, int año2)
    {
      try
      {
        var resultados = new List<ComparativoAnualDto>();
        var años = new[] { año1, año2 };

        foreach (var año in años.Distinct().OrderByDescending(y => y))
        {
          // Obtener total de Bienes aprobados para el año
          var totalBienes = await _context.Bienes
              .Where(b => b.Fecha.Year == año && b.StatusId == 3) // Filtro por status_ID = 3
              .SumAsync(b => b.Total);

          // Obtener total de Gastos aprobados para el año
          var totalGastos = await _context.Gasto
              .Where(g => g.Fecha.Year == año && g.StatusId == 3) // Filtro por status_ID = 3
              .SumAsync(g => g.Total);

          // Obtener total de Proyectos aprobados para el año
          var totalProyectos = await _context.Proyectos
              .Where(p => p.Fecha.Year == año && p.StatusId == 3) // Filtro por status_ID = 3
              .SumAsync(p => p.ValorEstimado);

          resultados.Add(new ComparativoAnualDto
          {
            Año = año,
            TotalBienes = totalBienes,
            TotalGastos = totalGastos,
            TotalProyectos = totalProyectos,
            TotalGeneral = totalBienes + totalGastos + totalProyectos
          });
        }

        return Ok(resultados);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = $"Error interno del servidor: {ex.Message}" });
      }
    }

    public class ComparativoAnualDto
    {
      public int Año { get; set; }
      public decimal TotalBienes { get; set; }
      public decimal TotalGastos { get; set; }
      public decimal TotalProyectos { get; set; }
      public decimal TotalGeneral { get; set; }
    }
  }
}
