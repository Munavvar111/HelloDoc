using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.InterFace;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace BusinessLayer.Repository
{
	public class EmailServiceRepository:IEmailServices
	{
		private readonly IConfiguration _configuration;
		private readonly IHostingEnvironment _hostingEnvironment;


		public EmailServiceRepository(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
		{
			_configuration = configuration;
			_hostingEnvironment = hostingEnvironment;
		}

		public bool IsSendEmail(string toEmail, string subject, string body, List<string> filenames)
		{
			try
			{
				IConfiguration emailSettings = _configuration.GetSection("EmailSettings");
				MimeMessage message = new MimeMessage();
				MailboxAddress from = new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]);
				MailboxAddress to = new MailboxAddress("", toEmail);
				message.From.Add(from);
				message.To.Add(to);
				message.Subject = subject;

				BodyBuilder bodyBuilder = new BodyBuilder();
				bodyBuilder.HtmlBody = body;
				if (filenames.Count > 0)
				{


					foreach (string filename in filenames)
					{
						string filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot/uploads", filename);

						MimePart attachment = new MimePart("application", "octet-stream")
						{
							Content = new MimeContent(File.OpenRead(filePath), ContentEncoding.Default),
							ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
							ContentTransferEncoding = ContentEncoding.Base64,
							FileName = filename
						};
						bodyBuilder.Attachments.Add(attachment);
					}
					message.Body = bodyBuilder.ToMessageBody();
					using (SmtpClient client = new SmtpClient())
					{

						client.Connect(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]));
						client.Authenticate(emailSettings["SmtpUsername"], emailSettings["SmtpPassword"]);
						client.Send(message);
						client.Disconnect(true);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error sending email: {ex.Message}");
				return false;
			}
		}
	}
}
