using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace WebApp.Models
{
    public class EmailConfig
    {
        private MailAddress _recipient;
        private MailAddress _sender;
        private string _subject;
        private string _body;
        private DateTime _created;
        private string _password;
        private SmtpClient _smtpClient;

        public EmailConfig(string recipient, string subject, string body)
        {
            _sender = new MailAddress("21555848@dut4life.ac.za", "Admin");
            _recipient = new MailAddress(recipient);
            _subject = subject;
            _body = body;
            _created = DateTime.Now;
            _password = "Powerhouse1!";
            _smtpClient = new SmtpClient
            {
                Host = "smtp.office365.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_sender.Address, _password)
            };
        }

        public EmailConfig()
        {
            _sender = new MailAddress("21555848@dut4life.ac.za", "Admin");
            _created = DateTime.Now;
            _password = "Powerhouse1!";
            _smtpClient = new SmtpClient
            {
                Host = "smtp.office365.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_sender.Address, _password)
            };
        }

        public bool SendEmail()
        {
            bool status = false;    
            using (var message = new MailMessage(_sender, _recipient)
            {
                Subject = _subject,
                Body = _body,
                IsBodyHtml = true
            })
            {
                try
                {
                    _smtpClient.Send(message);
                    status = true;
                    return status;
                }
                catch(Exception e)
                {
                    return status;
                }
                

            }
        }

        public bool SendEmail(string toEmail, string subject, string body)
        {
            bool status = false;
            var recipient = new MailAddress(toEmail);
            using (var message = new MailMessage(_sender, recipient)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml=true
            })
            {
                try
                {
                    _smtpClient.Send(message);
                    status = true;
                    return status;
                }
                catch (Exception e)
                {
                    return status;
                }


            }
        }
    }


}
