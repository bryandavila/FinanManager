using System.Net.Mail;
using System.Net;

namespace FinanManager.Services
{
  public interface IEmailSender
  {
    void SendEmail(string to, string subject, string body, string attachmentPath = null, string attachmentName = null);
  }

  public class EmailSender : IEmailSender
  {
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public void SendEmail(string to, string subject, string body, string attachmentPath = null, string attachmentName = null)
    {
      var smtpSettings = _configuration.GetSection("SmtpSettings");

      using (var client = new SmtpClient())
      {
        client.Host = smtpSettings["Server"];
        client.Port = int.Parse(smtpSettings["Port"]);
        client.EnableSsl = bool.Parse(smtpSettings["EnableSsl"]);
        client.Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]);

        var mailMessage = new MailMessage
        {
          From = new MailAddress(smtpSettings["SenderEmail"], smtpSettings["SenderName"]),
          Subject = subject,
          Body = body,
          IsBodyHtml = true
        };
        mailMessage.To.Add(to);

        if (!string.IsNullOrEmpty(attachmentPath) && System.IO.File.Exists(attachmentPath))
        {
          var attachment = new Attachment(attachmentPath);
          if (!string.IsNullOrEmpty(attachmentName))
          {
            attachment.Name = attachmentName; // Usar el nombre proporcionado
          }
          mailMessage.Attachments.Add(attachment);

          client.Send(mailMessage);

          // Cerrar el archivo adjunto después de su uso
          attachment.Dispose();
        }
        else
        {
          client.Send(mailMessage);
        }
      }
    }
  }
}
