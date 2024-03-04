using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace BusinessLayer.Repository
{
    public class AdminRepository:IAdmin
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        public AdminRepository(ApplicationDbContext context, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _context = context;

            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public List<NewRequestTableVM> SearchPatients(string searchValue, string selectValue, string selectedFilter, int[] currentStatus)
        {
                var filteredPatients = (from req in _context.Requests
                                    join reqclient in _context.Requestclients
                                    on req.Requestid equals reqclient.Requestid
                                    join p in _context.Physicians
                                    on req.Physicianid equals p.Physicianid into phy
                                    from ps in phy.DefaultIfEmpty()
                                    select new NewRequestTableVM
                                    {
                                        PatientName = reqclient.Firstname,
                                        Requestor = req.Firstname + " " + req.Lastname,
                                        DateOfBirth = new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                                        ReqDate = req.Createddate,
                                        Phone = reqclient.Phonenumber,
                                        PhoneOther = req.Phonenumber,
                                        Address = reqclient.City + reqclient.Zipcode,
                                        Notes = reqclient.Notes,
                                        ReqTypeId = req.Requesttypeid,
                                        Email = req.Email,
                                        Id = reqclient.Requestclientid,
                                        regionid = reqclient.Regionid,
                                        Status = req.Status,
                                        RequestClientId = reqclient.Requestclientid,
                                        RequestId = reqclient.Requestid,
                                        PhysicianName = ps.Firstname + "_" + ps.Lastname,
                                        Cancel = _context.Casetags.Select(cc => new CancelCase
                                        {
                                            CancelCaseReson = cc.Name,
                                            CancelReasonId = cc.Casetagid
                                        }).ToList()
                                    })
                                    .Where(item =>
                                        (string.IsNullOrEmpty(searchValue) || item.PatientName.Contains(searchValue)) &&
                                        (string.IsNullOrEmpty(selectValue) || item.regionid == int.Parse(selectValue)) &&
                                        (string.IsNullOrEmpty(selectedFilter) || item.ReqTypeId == int.Parse(selectedFilter)) &&
                                        currentStatus.Any(status=>item.Status==status)).ToList();
           
            return filteredPatients;
        }
        public List<NewRequestTableVM> GetAllData()
        {
            
            var GetAllData= from req in _context.Requests
                            join reqclient in _context.Requestclients
                            on req.Requestid equals reqclient.Requestid
                            join p in _context.Physicians
                                    on req.Physicianid equals p.Physicianid into phy
                            from ps in phy.DefaultIfEmpty()
                            select new NewRequestTableVM
                            {
                                PatientName = reqclient.Firstname,
                                Requestor = req.Firstname + " " + req.Lastname,
                                DateOfBirth = new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                                ReqDate = req.Createddate,
                                Phone = reqclient.Phonenumber,
                                Address = reqclient.City + reqclient.Zipcode,
                                Notes = reqclient.Notes,
                                ReqTypeId = req.Requesttypeid,
                                Email = req.Email,
                                Id = reqclient.Requestclientid,
                                Status = req.Status,
                                PhoneOther = req.Phonenumber,
                                RequestClientId = reqclient.Requestclientid,
                                RequestId = reqclient.Requestid,
                                PhysicianName = ps.Firstname + "_" + ps.Lastname,
                                Cancel = _context.Casetags.Select(cc => new CancelCase
                                {
                                    CancelCaseReson = cc.Name,
                                    CancelReasonId = cc.Casetagid
                                }).ToList() 
                            };
            return GetAllData.ToList();
        }
        public ViewCaseVM GetCaseById(int id)
        {
            var requestclient = (from req in _context.Requests
                                 join reqclient in _context.Requestclients
                                 on req.Requestid equals reqclient.Requestid
                                 where reqclient.Requestclientid == id
                                 select new ViewCaseVM
                                 {
                                     status=req.Status,
                                     RequestId=req.Requestid,
                                     Notes = reqclient.Notes,
                                     FirstName = reqclient.Firstname,
                                     LastName = reqclient.Lastname,
                                     DateOfBirth = new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                                     Phone = reqclient.Phonenumber,
                                     Email = reqclient.Email,
                                     Location = reqclient.Location,
                                     RequestClientId = reqclient.Requestclientid,
                                 }).FirstOrDefault();

            return requestclient;
        }

        public async Task UpdateRequestClient(ViewCaseVM viewCaseVM, int id)
        {
            var requestclient = await _context.Requestclients.FindAsync(id);
            if (requestclient != null)
            {
                requestclient.Firstname = viewCaseVM.FirstName;
                requestclient.Lastname = viewCaseVM.LastName;
                requestclient.Location = viewCaseVM.Location;
                requestclient.Email = viewCaseVM.Email;
                requestclient.Notes = viewCaseVM.Notes;
                requestclient.Phonenumber = viewCaseVM.Phone;
                requestclient.Intdate = viewCaseVM.DateOfBirth.Day;
                requestclient.Intyear = viewCaseVM.DateOfBirth.Year;
                requestclient.Strmonth = viewCaseVM.DateOfBirth.Month.ToString();

                _context.Update(requestclient);
                await _context.SaveChangesAsync();
            }
        }
        public List<ViewNotesVM> GetNotesForRequest(int requestid)
        {
            var leftJoin = from rn in _context.Requestnotes
                           join rs in _context.Requeststatuslogs on rn.Requestid equals rs.Requestid into rsJoin
                           from rs in rsJoin.DefaultIfEmpty()
                           join a in _context.Admins on rs.Adminid equals a.Adminid into aJoin
                           from a in aJoin.DefaultIfEmpty()
                           join p in _context.Physicians on rs.Physicianid equals p.Physicianid into pJoin
                           from p in pJoin.DefaultIfEmpty()
                           where rn.Requestid == requestid
                           select new ViewNotesVM
                           {
                               TransToPhysicianId = rs.Transtophysicianid,
                               Status = rs.Status,
                               AdminName = a.Firstname ?? "" + " " + a.Lastname ?? "",
                               PhysicianName = p.Firstname ?? "" + " " + p.Lastname ?? "",
                               AdminNotes = rn.Adminnotes,
                               PhysicianNotes = rn.Physiciannotes,
                               TransferNotes = rs.Notes,
                               Cancelcount = _context.Requeststatuslogs.Count(item => item.Status == 5)
                           };

            var rightJoin = from rs in _context.Requeststatuslogs
                            join rn in _context.Requestnotes on rs.Requestid equals rn.Requestid into rnJoin
                            from rn in rnJoin.DefaultIfEmpty()
                            join a in _context.Admins on rs.Adminid equals a.Adminid into aJoin
                            from a in aJoin.DefaultIfEmpty()
                            join p in _context.Physicians on rs.Physicianid equals p.Physicianid into pJoin
                            from p in pJoin.DefaultIfEmpty()
                            where rs.Requestid == requestid && rn == null // Filter only records not in left join result
                            select new ViewNotesVM
                            {
                                TransToPhysicianId = rs.Transtophysicianid,
                                Status = rs.Status,
                                AdminName = a.Firstname ?? "" + " " + a.Lastname ?? "",
                                PhysicianName = p.Firstname ?? "" + " " + p.Lastname ?? "",
                                AdminNotes = rn.Adminnotes,
                                PhysicianNotes = rn.Physiciannotes,
                                TransferNotes = rs.Notes,
                                Cancelcount = _context.Requeststatuslogs.Count(item => item.Status == 5)
                            };
            var result = leftJoin.Union(rightJoin).ToList();
            return result;

        }

        public async Task<bool> AssignRequest(int regionId, int physician, string description, int requestId)
        {
            try
            {
                var request = await _context.Requests.FindAsync(requestId);

                if (request != null)
                {
                    request.Status = 2;
                    request.Physicianid = physician;
                    _context.Requests.Update(request);

                    var requestStatusLog = new Requeststatuslog
                    {
                        Notes = description,
                        Requestid = requestId,
                        Status = 2,
                        Createddate = DateTime.Now
                    };

                    _context.Requeststatuslogs.Add(requestStatusLog);
                    await _context.SaveChangesAsync();

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                // Handle exceptions according to your application's needs
                return false;
            }
        }

        public async Task<bool> UpdateAdminNotes(int requestId, string adminNotes)
        {
            try
            {
                var requestVisenotes = await _context.Requestnotes.FirstOrDefaultAsync(item => item.Requestid == requestId);

                if (requestVisenotes != null)
                {
                    requestVisenotes.Adminnotes = adminNotes;
                    _context.Update(requestVisenotes);
                }
                else
                {
                    var requestNotes = new Requestnote
                    {
                        Requestid = requestId,
                        Adminnotes = adminNotes,
                        Createddate = DateTime.Now,
                        Createdby = "admin"
                    };

                    _context.Add(requestNotes);
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                // Handle exceptions according to your application's needs
                return false;
            }
        }

        public async Task<bool> CancelCase(int requestId, string notes, string cancelReason)
        {
            try
            {
                var requestById = await _context.Requests.FindAsync(requestId);

                if (requestById != null)
                {
                    requestById.Status = 3;
                    _context.Requests.Update(requestById);

                    var requestStatusLog = new Requeststatuslog
                    {
                        Status = 3,
                        Requestid = requestId,
                        Notes = notes,
                        Createddate = DateTime.Now
                    };

                    _context.Requeststatuslogs.Add(requestStatusLog);
                    await _context.SaveChangesAsync();

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                // Handle exceptions according to your application's needs
                return false;
            }
        }

        public bool IsSendEmail(string toEmail, string subject, string body, List<string> filenames)
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

                foreach (var filename in filenames)
                {
                    // Attach the files to the email
                    var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot/uploads", filename);

                    // Create an attachment
                    var attachment = new MimePart("application", "octet-stream")
                    {
                        Content = new MimeContent(File.OpenRead(filePath), ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = filename
                    };

                    // Add the attachment to the email
                    bodyBuilder.Attachments.Add(attachment);

                    // Update IsDeleted using BitArray
                    var file = _context.Requestwisefiles.FirstOrDefault(item => item.Filename == filename);
                    
                }

                // Set the email body
                message.Body = bodyBuilder.ToMessageBody();

                // Use your existing SmtpClient logic to send the email
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

    }
}
