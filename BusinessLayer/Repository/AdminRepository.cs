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
        #region SearchPatients
        public List<NewRequestTableVM> SearchPatients(string searchValue, string selectValue, string selectedFilter, int[] currentStatus)
        {
            var filteredPatients = (from req in _context.Requests
                                    join reqclient in _context.Requestclients
                                    on req.Requestid equals reqclient.Requestid
                                    join p in _context.Physicians
                                    on req.Physicianid equals p.Physicianid into phy
                                    from ps in phy.DefaultIfEmpty()
                                    join encounter in _context.Encounterforms
                                    on req.Requestid equals encounter.RequestId into enco
                                    from eno in enco.Where(e => e != null).DefaultIfEmpty()
                                    select new NewRequestTableVM
                                    {
                                        PatientName = reqclient.Firstname.ToLower(),
                                        Requestor = req.Firstname + " " + req.Lastname,
                                        DateOfBirth = new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                                        ReqDate = req.Createddate,
                                        Phone = reqclient.Phonenumber??"",
                                        PhoneOther = req.Phonenumber ?? "",
                                        Address = reqclient.City + reqclient.Zipcode,
                                        Notes = reqclient.Notes ?? "",
                                        ReqTypeId = req.Requesttypeid,
                                        Email = req.Email ?? "",
                                        Id = reqclient.Requestclientid,
                                        regionid = reqclient.Regionid,
                                        Status = req.Status,
                                        RequestClientId = reqclient.Requestclientid,
                                        RequestId = reqclient.Requestid,
                                        isfinalize = eno.IsFinalize,
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
        #endregion

        #region GetAllDataTable
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
                                Phone = reqclient.Phonenumber ?? "",
                                Address = reqclient.City + reqclient.Zipcode,
                                Notes = reqclient.Notes ?? "",
                                ReqTypeId = req.Requesttypeid,
                                Email = req.Email ?? "",
                                Id = reqclient.Requestclientid,
                                Status = req.Status,
                                PhoneOther = req.Phonenumber ?? "",
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
        #endregion

        #region ViewCaseById
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
                                     LastName = reqclient.Lastname ?? "",
                                     DateOfBirth = new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                                     Phone = reqclient.Phonenumber ?? "",
                                     EmailView = reqclient.Email ?? "",
                                     Location = reqclient.Location,
                                     RequestClientId = reqclient.Requestclientid,
                                     Cancel = _context.Casetags.Select(cc => new CancelCase
                                     {
                                         CancelCaseReson = cc.Name,
                                         CancelReasonId = cc.Casetagid
                                     }).ToList()
                                 }).FirstOrDefault();

            return requestclient;
        }
        #endregion

        #region UpdateViewCase
        public async Task UpdateRequestClient(ViewCaseVM viewCaseVM, int id)
        {
            Requestclient? requestclient = await _context.Requestclients.FindAsync(id);
            if (requestclient != null)
            {
                requestclient.Firstname = viewCaseVM.FirstName;
                requestclient.Lastname = viewCaseVM.LastName;
                requestclient.Location = viewCaseVM.Location;
                requestclient.Email = viewCaseVM.EmailView;
                requestclient.Notes = viewCaseVM.Notes;
                requestclient.Phonenumber = viewCaseVM.Phone;
                requestclient.Intdate = viewCaseVM.DateOfBirth.Day;
                requestclient.Intyear = viewCaseVM.DateOfBirth.Year;
                requestclient.Strmonth = viewCaseVM.DateOfBirth.Month.ToString();

                _context.Update(requestclient);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region GetNotes
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
                               Cancelcount = _context.Requeststatuslogs.Count(item => item.Status == 3 || item.Status==7)
                           };

            var rightJoin = from rs in _context.Requeststatuslogs
                            join rn in _context.Requestnotes on rs.Requestid equals rn.Requestid into rnJoin
                            from rn in rnJoin.DefaultIfEmpty()
                            join a in _context.Admins on rs.Adminid equals a.Adminid into aJoin
                            from a in aJoin.DefaultIfEmpty()
                            join p in _context.Physicians on rs.Physicianid equals p.Physicianid into pJoin
                            from p in pJoin.DefaultIfEmpty()
                            where rs.Requestid == requestid // Filter only records not in left join result
                            select new ViewNotesVM
                            {
                                TransToPhysicianId = rs.Transtophysicianid,
                                Status = rs.Status,
                                AdminName = a.Firstname ?? "" + " " + a.Lastname ?? "",
                                PhysicianName = p.Firstname ?? "" + " " + p.Lastname ?? "",
                                AdminNotes = rn.Adminnotes,
                                PhysicianNotes = rn.Physiciannotes,
                                TransferNotes = rs.Notes,
                                Cancelcount = _context.Requeststatuslogs.Count(item => item.Status == 3 || item.Status==7)
                            };
            List<ViewNotesVM> result = leftJoin.Union(rightJoin).ToList();
            return result;
        }
        #endregion

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
                    var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot/uploads", filename);

                    var attachment = new MimePart("application", "octet-stream")
                    {
                        Content = new MimeContent(File.OpenRead(filePath), ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = filename
                    };
                    bodyBuilder.Attachments.Add(attachment);
               }
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
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
        public bool BlockRequest(string blockReason, int requestId)
        {
            try
            {
                var request = _context.Requests.Find(requestId);

                if (request != null)
                {
                    request.Status = 11;
                    _context.Requests.Update(request);

                    var block = new Blockrequest
                    {
                        Email = request.Email,
                        Phonenumber = request.Phonenumber,
                        Requestid = requestId.ToString(),
                        Createddate = DateTime.Now,
                        Reason = blockReason
                    };

                    _context.Blockrequests.Add(block);

                    var requeststatuslog = new Requeststatuslog
                    {
                        Status = 11,
                        Createddate = DateTime.Now,
                        Requestid = requestId,
                        Notes = blockReason
                    };

                    _context.Requeststatuslogs.Add(requeststatuslog);
                    _context.SaveChanges();

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SendOrders(SendOrderModel order)
        {
            try
            {
                var orderdetail = new Orderdetail
                {
                    Requestid = order.requestid,
                    Email = order.Email,
                    Prescription = order.Prescription,
                    Vendorid = order.BusinessId,
                    Faxnumber = order.FaxNumber,
                    Businesscontact = order.Contact
                };

                _context.Orderdetails.Add(orderdetail);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public SendOrderModel GetSendOrder(int requestid)
        {
            var profession = _context.Healthprofessionaltypes.ToList();
            var business = _context.Healthprofessionals.ToList();

            var sendorder = new SendOrderModel
            {
                requestid = requestid,
                Healthprofessionaltypes = profession,
                helthProfessional = business
            };

            return sendorder;
        }
        public ViewEncounterForm GetEncounterForm(int requestid)
        {
            var encounterformbyrequestid = _context.Encounterforms
                .Where(item => item.RequestId == requestid)
                .FirstOrDefault();

            var request = _context.Requestclients
                .Where(item => item.Requestid == requestid)
                .FirstOrDefault();

            var viewencounterform = new ViewEncounterForm();

            if (encounterformbyrequestid != null)
            {
                viewencounterform.FirstName = request.Firstname;
                viewencounterform.LastName = request.Lastname;
                viewencounterform.DateOfBirth = new DateOnly((int)request.Intyear, int.Parse(request.Strmonth), (int)request.Intdate);
                viewencounterform.Email = request.Email;
                viewencounterform.Location = $"{request.Address}, {request.City}, {request.State}";

                viewencounterform.ABD = encounterformbyrequestid.Abd;
                viewencounterform.Skin = encounterformbyrequestid.Skin;
                viewencounterform.RR = encounterformbyrequestid.Rr;
                viewencounterform.Procedures = encounterformbyrequestid.Procedures;
                viewencounterform.CV = encounterformbyrequestid.Cv;
                viewencounterform.Chest = encounterformbyrequestid.Chest;
                viewencounterform.Allergies = encounterformbyrequestid.Allergies;
                viewencounterform.BPDiastolic = encounterformbyrequestid.BloodPressureDiastolic;
                viewencounterform.BPSystolic = encounterformbyrequestid.BloodPressureSystolic;
                viewencounterform.Diagnosis = encounterformbyrequestid.Diagnosis;
                viewencounterform.Followup = encounterformbyrequestid.FollowUp;
                viewencounterform.Heent = encounterformbyrequestid.Heent;
                viewencounterform.HistoryOfPresentIllness = encounterformbyrequestid.HistoryOfPresentIllnessOrInjury;
                viewencounterform.HR = encounterformbyrequestid.Hr;
                viewencounterform.MedicalHistory = encounterformbyrequestid.MedicalHistory;
                viewencounterform.Medications = encounterformbyrequestid.Medications;
                viewencounterform.MedicationsDispensed = encounterformbyrequestid.MedicationsDispensed;
                viewencounterform.Neuro = encounterformbyrequestid.Neuro;
                viewencounterform.O2 = encounterformbyrequestid.O2;
                viewencounterform.Other = encounterformbyrequestid.Other;
                viewencounterform.Pain = encounterformbyrequestid.Pain;
                viewencounterform.Temperature = encounterformbyrequestid.Temp;
                viewencounterform.TreatmentPlan = encounterformbyrequestid.TreatmentPlan;
            }

            return viewencounterform;
        }

        public void SaveOrUpdateEncounterForm(ViewEncounterForm viewEncounterForm, string requestid)
    {
        var encounter = _context.Encounterforms
            .FirstOrDefault(item => item.RequestId == int.Parse(requestid));

        if (encounter == null)
        {
            AddNewEncounterForm(viewEncounterForm, requestid);
        }
        else
        {
            UpdateExistingEncounterForm(encounter, viewEncounterForm, requestid);
        }
    }

    private void AddNewEncounterForm(ViewEncounterForm viewEncounterForm, string requestid)
    {
        var encounterFirsttime = new Encounterform
        {
            Abd = viewEncounterForm.ABD,
            Skin = viewEncounterForm.Skin,
            Rr = viewEncounterForm.RR,
            Procedures = viewEncounterForm.Procedures,
            Cv = viewEncounterForm.CV,
            Chest = viewEncounterForm.Chest,
            Allergies = viewEncounterForm.Allergies,
            BloodPressureDiastolic = viewEncounterForm.BPDiastolic,
            BloodPressureSystolic = viewEncounterForm.BPSystolic,
            Diagnosis = viewEncounterForm.Diagnosis,
            FollowUp = viewEncounterForm.Followup,
            RequestId = int.Parse(requestid),
            Heent = viewEncounterForm.Heent,
            HistoryOfPresentIllnessOrInjury = viewEncounterForm.HistoryOfPresentIllness,
            Hr = viewEncounterForm.HR,
            IsFinalize = false,
            MedicalHistory = viewEncounterForm.MedicalHistory,
            Medications = viewEncounterForm.Medications,
            MedicationsDispensed = viewEncounterForm.MedicationsDispensed,
            Neuro = viewEncounterForm.Neuro,
            O2 = viewEncounterForm.O2,
            Other = viewEncounterForm.Other,
            Pain = viewEncounterForm.Pain,
            Temp = viewEncounterForm.Temperature,
            TreatmentPlan = viewEncounterForm.TreatmentPlan
        };

        _context.Encounterforms.Add(encounterFirsttime);
        _context.SaveChanges();
    }

    private void UpdateExistingEncounterForm(Encounterform encounter, ViewEncounterForm viewEncounterForm, string requestid)
    {
        encounter.Abd = viewEncounterForm.ABD;
        encounter.Skin = viewEncounterForm.Skin;
        encounter.Rr = viewEncounterForm.RR;
        encounter.Procedures = viewEncounterForm.Procedures;
        encounter.Cv = viewEncounterForm.CV;
        encounter.Chest = viewEncounterForm.Chest;
        encounter.Allergies = viewEncounterForm.Allergies;
        encounter.BloodPressureDiastolic = viewEncounterForm.BPDiastolic;
        encounter.BloodPressureSystolic = viewEncounterForm.BPSystolic;
        encounter.Diagnosis = viewEncounterForm.Diagnosis;
        encounter.FollowUp = viewEncounterForm.Followup;
        encounter.RequestId = int.Parse(requestid);
        encounter.Heent = viewEncounterForm.Heent;
        encounter.HistoryOfPresentIllnessOrInjury = viewEncounterForm.HistoryOfPresentIllness;
        encounter.Hr = viewEncounterForm.HR;
        encounter.IsFinalize = false;
        encounter.MedicalHistory = viewEncounterForm.MedicalHistory;
        encounter.Medications = viewEncounterForm.Medications;
        encounter.MedicationsDispensed = viewEncounterForm.MedicationsDispensed;
        encounter.Neuro = viewEncounterForm.Neuro;
        encounter.O2 = viewEncounterForm.O2;
        encounter.Other = viewEncounterForm.Other;
        encounter.Pain = viewEncounterForm.Pain;
        encounter.Procedures = viewEncounterForm.Procedures;
        encounter.Temp = viewEncounterForm.Temperature;
        encounter.TreatmentPlan = viewEncounterForm.TreatmentPlan;

        _context.Encounterforms.Update(encounter);
        _context.SaveChanges();
    }
    }
}
