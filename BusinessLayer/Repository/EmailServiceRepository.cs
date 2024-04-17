using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.InterFace;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace BusinessLayer.Repository
{
	public class EmailServiceRepository:IEmailServices
	{
		private readonly IConfiguration _configuration;
		[Obsolete]
		private readonly IHostingEnvironment _hostingEnvironment;
		private readonly ApplicationDbContext _context;

		[Obsolete]
		public EmailServiceRepository(IConfiguration configuration, IHostingEnvironment hostingEnvironment,ApplicationDbContext context)
		{
			_configuration = configuration;
			_hostingEnvironment = hostingEnvironment;
			_context = context;	
		}

		[Obsolete]
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
		public void EmailLog(string email, string message, string subject, string? name, int? roleId, int? requestId, int? adminId, int? physicianId, int action, bool isSent, int sentTries)
		{
			try
			{
				Emaillog log = new Emaillog();
				log.Emailtemplate = message;
				log.Subjectname = subject;
				log.Emailid = email;
				log.Roleid = roleId;
				log.Createdate = DateTime.Now;
				log.Sentdate = DateTime.Now;
				log.Adminid = adminId;
				log.Requestid = requestId;
				log.Physicianid = physicianId;
				log.Action = action;
				log.Receivername = name;

				if (requestId != null)
				{
					Request? request = _context.Requests.FirstOrDefault(r => r.Requestid == requestId);
					if (request != null && request.Confirmationnumber != null)
					{
						log.Confirmationnumber = request.Confirmationnumber;
					}
				}

				if (isSent)
				{
					log.Isemailsent = true;
				}
				else
				{
					log.Isemailsent = false;
				}
				log.Senttries = sentTries;
				_context.Emaillogs.Add(log);
				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Failed to submit Form", ex);
			}
		}
	}
}
