using MimeKit;
using MimeKit.Text;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

public static class EmailService {
     
      public static void Send(string to, string subject, string html,  IConfiguration _configuration, ILogger<FileReader> _logger,string from = null)
    {
        try{
        // create message
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from ??  _configuration["FileConfig:EmailFrom"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = html };

        // send email
        using var smtp = new SmtpClient();
        var smtpPort = Convert.ToInt32( _configuration["FileConfig:SmtpPort"]);
        smtp.Connect(_configuration["FileConfig:SmtpHost"],  smtpPort, SecureSocketOptions.StartTls);
        smtp.Authenticate(_configuration["FileConfig:SmtpUser"], _configuration["FileConfig:SmtpPass"]);
        smtp.Send(email);
        smtp.Disconnect(true); 
        } catch (Exception ex) {
            _logger.LogError(ex, ex.Message);
        }
    }
}