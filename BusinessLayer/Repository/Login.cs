using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;
using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Authentication;
using MailKit.Security;

namespace BusinessLayer.Repository
{
    public class Login : ILogin
    {   
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public Login(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public bool IsSendEmail(string toEmail, string subject, string body)
        {
            try
            {

            var emailSettings = _configuration.GetSection("EmailSettings");
            var message = new MimeMessage();
            var from = new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]);
            var to = new MailboxAddress("", toEmail);
            message.From.Add(from);
            message.To.Add(to);
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                    client.Connect(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]));
                client.Authenticate(emailSettings["SmtpUsername"], emailSettings["SmtpPassword"]);
                client.Send(message);
                client.Disconnect(true);
            }
            return true;
            }
            catch (Exception ex)
            {
                // Handle exceptions here, you can log the exception for debugging purposes
                // and return false to indicate that the email sending failed
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }


        public bool isLoginValid(LoginModel a)
        {
            var account = _context.AspnetUsers.SingleOrDefault(x => x.Email == a.Email);

            // check account found and verify password
            if (account == null || !BC.Verify(a.Passwordhash, account.Passwordhash))
            {
                // authentication failed
                return false;
            }
            else
            {
                // authentication successful
                return true;
            }
        }
    }
}
