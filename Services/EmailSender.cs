using System.Net.Mail;
using System.Net;

namespace FinanManager.Services
{
  public interface IEmailSender
  {
    void SendEmail(string to, string subject, string body);
  }

  public class EmailSender : IEmailSender
  {
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public void SendEmail(string to, string subject, string body)
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

        client.Send(mailMessage);
      }
    }
  }
}
