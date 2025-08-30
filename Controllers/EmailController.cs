using Microsoft.AspNetCore.Mvc;
using FinanManager.Services;
using System.IO;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Configuration;
using System;

namespace FinanManager.Controllers
{
  [ApiController]
  [Route("api/email")]
  public class EmailController : ControllerBase
  {
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public EmailController(IEmailSender emailSender, IConfiguration configuration)
    {
      _emailSender = emailSender;
      _configuration = configuration;
    }

    [HttpPost("send-report")]
    public async Task<IActionResult> SendReport([FromForm] IFormFile file, [FromForm] string email, [FromForm] string dateTime, [FromForm] string reportFormat)
    {
      string filePath = null;
      try
      {
        // Guardar el archivo temporalmente
        filePath = Path.GetTempFileName();
        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
          await file.CopyToAsync(stream);
        }

        // Verificar que el archivo se haya creado correctamente
        if (!System.IO.File.Exists(filePath))
        {
          return StatusCode(500, new { success = false, error = "No se pudo crear el archivo temporal." });
        }

        // Determinar el nombre del archivo adjunto según el formato
        string attachmentName = reportFormat == "excel" ? "reporte.xlsx" : "reporte.pdf";

        // Enviar el correo electrónico
        var subject = "Reporte Automático de Gastos";
        var message = $"Se ha generado un reporte automático en formato {reportFormat}. Fecha y hora de envío: {dateTime}.";
        _emailSender.SendEmail(email, subject, message, filePath, attachmentName);

        return Ok(new { success = true });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { success = false, error = ex.Message });
      }
      finally
      {
        // Eliminar el archivo temporal después de su uso
        if (filePath != null && System.IO.File.Exists(filePath))
        {
          try
          {
            System.IO.File.Delete(filePath);
          }
          catch (IOException)
          {
            // Si el archivo está en uso, no se puede eliminar. Se puede ignorar o registrar el error.
            Console.WriteLine("No se pudo eliminar el archivo temporal porque está en uso.");
          }
        }
      }
    }

    [HttpPost("schedule-report")]
    public IActionResult ScheduleReport([FromForm] IFormFile file, [FromForm] string email, [FromForm] string dateTime, [FromForm] string reportFormat)
    {
      try
      {
        // Validar que la fecha y hora sean en el futuro
        var scheduledDateTime = DateTime.Parse(dateTime);
        if (scheduledDateTime <= DateTime.Now)
        {
          return BadRequest(new { success = false, error = "La fecha y hora deben ser en el futuro." });
        }

        // Guardar el archivo temporalmente
        var filePath = Path.GetTempFileName();
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          file.CopyTo(stream);
        }

        // Programar el envío del correo
        var jobId = BackgroundJob.Schedule(() => SendScheduledEmail(email, filePath, reportFormat), scheduledDateTime);

        return Ok(new { success = true, jobId });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { success = false, error = ex.Message });
      }
    }

    public void SendScheduledEmail(string email, string filePath, string reportFormat)
    {
      var subject = "Reporte Automático de Gastos";
      var message = $"Se ha generado un reporte automático en formato {reportFormat}. Fecha y hora de envío: {DateTime.Now}.";

      var emailSender = new EmailSender(_configuration);
      emailSender.SendEmail(email, subject, message, filePath, reportFormat == "excel" ? "reporte.xlsx" : "reporte.pdf");

      // Eliminar el archivo temporal después de su uso
      if (System.IO.File.Exists(filePath))
      {
        System.IO.File.Delete(filePath);
      }
    }
  }
}
