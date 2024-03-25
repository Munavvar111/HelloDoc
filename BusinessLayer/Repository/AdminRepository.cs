using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using BC = BCrypt.Net.BCrypt;
using System.Net;
using System.Collections;

namespace BusinessLayer.Repository
{
    public class AdminRepository : IAdmin
    {
        private readonly ApplicationDbContext _context;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IUploadProvider _uploadprovider;

            
        public AdminRepository(ApplicationDbContext context,IUploadProvider uploadProvider, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _context = context;

            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _uploadprovider = uploadProvider;
        }

        public Physician GetPhysicianByEmail(string email)
        {
            return _context.Physicians.FirstOrDefault(item => item.Email == email);
        }
        public Admin GetAdminByEmail(string email)
        {
            return _context.Admins.FirstOrDefault(item => item.Email == email);
        }

        public AspnetUser GetAspNetUserByEmail(string email)
        {
            return _context.AspnetUsers.FirstOrDefault(item => item.Email == email);
        }
        public List<int> GetUserPermissions(string roleid)
        {
            var menulist = _context.Rolemenus.Where(item => item.Roleid == int.Parse(roleid)).Select(item => item.Menuid).ToList();
            ;
            return menulist;
        }
        public Menu GetMenufromMenuid(string menuid)
        {
            var menu = _context.Menus.Where(item => item.Menuid == int.Parse(menuid)).FirstOrDefault();
            return menu;
        }

        #region SearchPatients
        public List<NewRequestTableVM> SearchPatients(string searchValue, string selectValue, string selectedFilter, int[] currentStatus)
        {
            List<NewRequestTableVM> filteredPatients = (from req in _context.Requests
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
                                                            DateOfBirth = reqclient != null && reqclient.Intyear != null && reqclient.Strmonth != null && reqclient.Intdate != null
                                                                            ? new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate)
                                                                            : new DateOnly(),
                                                            ReqDate = req.Createddate,
                                                            FirstName=reqclient.Firstname,
                                                            LastName=reqclient.Lastname,
                                                            Phone = reqclient.Phonenumber ?? "",
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
                                                            Regions = _context.Regions.ToList(),
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
                                        currentStatus.Any(status => item.Status == status)).ToList();

            return filteredPatients;
        }
        #endregion

        #region GetAllDataTable
        public List<NewRequestTableVM> GetAllData()
        {

            IQueryable<NewRequestTableVM> GetAllData = from req in _context.Requests
                                                       join reqclient in _context.Requestclients
                                                       on req.Requestid equals reqclient.Requestid
                                                       join p in _context.Physicians
                                                               on req.Physicianid equals p.Physicianid into phy
                                                       from ps in phy.DefaultIfEmpty()
                                                       select new NewRequestTableVM
                                                       {
                                                           PatientName = reqclient.Firstname,
                                                           Requestor = req.Firstname + " " + req.Lastname,
                                                           DateOfBirth = reqclient.Intyear != null && reqclient.Strmonth != null && reqclient.Intdate != null
                                                                                   ? new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate)
                                                                                   : new DateOnly(),
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
                                                           Regions = _context.Regions.ToList(),
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
            ViewCaseVM? requestclient = (from req in _context.Requests
                                         join reqclient in _context.Requestclients
                                         on req.Requestid equals reqclient.Requestid
                                         where reqclient.Requestclientid == id
                                         select new ViewCaseVM
                                         {
                                             status = req.Status,
                                             RequestId = req.Requestid,
                                             Notes = reqclient.Notes,
                                             FirstName = reqclient.Firstname,
                                             LastName = reqclient.Lastname ?? "",
                                             DateOfBirth = reqclient.Intyear != null && reqclient.Strmonth != null && reqclient.Intdate != null ? new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate) : new DateOnly(),
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

            if (requestclient != null)
            {
                return requestclient;
            }
            else
            {
                return new ViewCaseVM();
            }
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
            IQueryable<ViewNotesVM> leftJoin = from rn in _context.Requestnotes
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
                                                   Cancelcount = _context.Requeststatuslogs.Count(item => item.Status == 3 || item.Status == 7)
                                               };

            IQueryable<ViewNotesVM> rightJoin = from rs in _context.Requeststatuslogs
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
                                                    Cancelcount = _context.Requeststatuslogs.Count(item => item.Status == 3 || item.Status == 7)
                                                };
            List<ViewNotesVM> result = leftJoin.Union(rightJoin).ToList();
            return result;
        }
        #endregion

        #region AssignRequest
        public async Task<bool> AssignRequest(int regionId, int physician, string description, int requestId,int adminid)
        {
            try
            {
               
                Request? request = await _context.Requests.FindAsync(requestId);

                if (request != null)
                {
                    request.Status = 2;
                    request.Physicianid = physician;
                    _context.Requests.Update(request);

                    Requeststatuslog requestStatusLog = new Requeststatuslog
                    {
                        Adminid=adminid,
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
        #endregion

        #region UpdateAdminNotes
        public async Task<bool> UpdateAdminNotes(int requestId, string adminNotes)
        {
            try
            {
                Requestnote? requestVisenotes = await _context.Requestnotes.FirstOrDefaultAsync(item => item.Requestid == requestId);

                if (requestVisenotes != null)
                {
                    requestVisenotes.Adminnotes = adminNotes;
                    _context.Update(requestVisenotes);
                }
                else
                {
                    Requestnote requestNotes = new Requestnote
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
        #endregion

        #region CancelCase
        public async Task<bool> CancelCase(int requestId, string notes, string cancelReason)
        {
            try
            {
                Request? requestById = await _context.Requests.FindAsync(requestId);

                if (requestById != null)
                {
                    requestById.Status = 3;
                    requestById.Casetag = _context.Casetags.FirstOrDefault(c => c.Casetagid == int.Parse(cancelReason)).Name;
                    _context.Requests.Update(requestById);

                    Requeststatuslog requestStatusLog = new Requeststatuslog
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
        #endregion

        #region SendEmail
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

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region BLockRequest
        public bool BlockRequest(string blockReason, int requestId)
        {
            try
            {
                Request? request = _context.Requests.Find(requestId);

                if (request != null)
                {
                    request.Status = 11;
                    _context.Requests.Update(request);

                    Blockrequest block = new Blockrequest
                    {
                        Email = request.Email,
                        Phonenumber = request.Phonenumber,
                        Requestid = requestId.ToString(),
                        Createddate = DateTime.Now,
                        Reason = blockReason
                    };

                    _context.Blockrequests.Add(block);

                    Requeststatuslog requeststatuslog = new Requeststatuslog
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
        #endregion

        #region SendOrders
        public bool SendOrders(SendOrderModel order)
        {
            try
            {
                Orderdetail orderdetail = new Orderdetail
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
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        #endregion

        #region GetSendOrder
        public SendOrderModel GetSendOrder(int requestid)
        {
            List<Healthprofessionaltype> profession = _context.Healthprofessionaltypes.ToList();
            List<Healthprofessional> business = _context.Healthprofessionals.ToList();

            SendOrderModel sendorder = new SendOrderModel
            {
                requestid = requestid,
                Healthprofessionaltypes = profession,
                helthProfessional = business
            };

            return sendorder;
        }
        #endregion

        #region GetEncounterForm
        public ViewEncounterForm GetEncounterForm(int requestid)
        {
            Encounterform? encounterformbyrequestid = _context.Encounterforms
                .Where(item => item.RequestId == requestid)
                .FirstOrDefault();

            Requestclient? request = _context.Requestclients
                .Where(item => item.Requestid == requestid)
                .FirstOrDefault();

            ViewEncounterForm viewencounterform = new ViewEncounterForm();

            if (encounterformbyrequestid != null)
            {
                viewencounterform.FirstName = request?.Firstname ?? "";
                viewencounterform.LastName = request?.Lastname ?? "";
                viewencounterform.DateOfBirth = request?.Intyear != null && request.Strmonth != null && request.Intdate != null
                ? new DateOnly((int)request.Intyear, int.Parse(request.Strmonth), (int)request.Intdate)
                : new DateOnly();
                viewencounterform.Email = request?.Email ?? "";
                viewencounterform.Location = $"{request?.Address ?? ""}, {request?.City ?? ""}, {request?.State ?? ""}";

                viewencounterform.ABD = encounterformbyrequestid?.Abd ?? "";
                viewencounterform.Skin = encounterformbyrequestid?.Skin ?? "";
                viewencounterform.RR = encounterformbyrequestid?.Rr ?? "";
                viewencounterform.Procedures = encounterformbyrequestid?.Procedures ?? "";
                viewencounterform.CV = encounterformbyrequestid?.Cv ?? "";
                viewencounterform.Chest = encounterformbyrequestid?.Chest ?? "";
                viewencounterform.Allergies = encounterformbyrequestid?.Allergies ?? "";
                viewencounterform.BPDiastolic = encounterformbyrequestid?.BloodPressureDiastolic ?? "";
                viewencounterform.BPSystolic = encounterformbyrequestid?.BloodPressureSystolic ?? "";
                viewencounterform.Diagnosis = encounterformbyrequestid?.Diagnosis ?? "";
                viewencounterform.Followup = encounterformbyrequestid?.FollowUp ?? "";
                viewencounterform.Heent = encounterformbyrequestid?.Heent ?? "";
                viewencounterform.HistoryOfPresentIllness = encounterformbyrequestid?.HistoryOfPresentIllnessOrInjury ?? "";
                viewencounterform.HR = encounterformbyrequestid?.Hr ?? "";
                viewencounterform.MedicalHistory = encounterformbyrequestid?.MedicalHistory ?? "";
                viewencounterform.Medications = encounterformbyrequestid?.Medications ?? "";
                viewencounterform.MedicationsDispensed = encounterformbyrequestid?.MedicationsDispensed ?? "";
                viewencounterform.Neuro = encounterformbyrequestid?.Neuro ?? "";
                viewencounterform.O2 = encounterformbyrequestid?.O2 ?? "";
                viewencounterform.Other = encounterformbyrequestid?.Other ?? "";
                viewencounterform.Pain = encounterformbyrequestid?.Pain ?? "";
                viewencounterform.Temperature = encounterformbyrequestid?.Temp ?? "";
                viewencounterform.TreatmentPlan = encounterformbyrequestid?.TreatmentPlan ?? "";
            }

            return viewencounterform;
        }
        #endregion

        #region SaveOrUpdateEncounterForm
        public void SaveOrUpdateEncounterForm(ViewEncounterForm viewEncounterForm, string requestid)
        {
            Encounterform? encounter = _context.Encounterforms
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
        #endregion

        #region AddNewEncounterForm
        public void AddNewEncounterForm(ViewEncounterForm viewEncounterForm, string requestid)
        {
            Encounterform encounterFirsttime = new Encounterform
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
        #endregion

        #region UpdateExistingEncounterForm
        public void UpdateExistingEncounterForm(Encounterform encounter, ViewEncounterForm viewEncounterForm, string requestid)
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
        #endregion
        
        #region GetAdminProfile
        public AdminProfileVm GetAdminProfile(string email)
        {
            var admin = _context.Admins.FirstOrDefault(item => item.Email == email);
            var adminProfile = new AdminProfileVm();

            if (admin != null)
            {
                adminProfile.FirstName = admin.Firstname;
                adminProfile.LastName = admin.Lastname ?? "";
                adminProfile.Email = admin.Email;
                adminProfile.Address1 = admin.Address1 ?? "";
                adminProfile.Address2 = admin.Address2 ?? "";
                adminProfile.City = admin.City ?? "";
                adminProfile.ZipCode = admin.Zip ?? "";
                adminProfile.MobileNo = admin.Mobile ?? "";
                adminProfile.Regions = _context.Regions.ToList();
                adminProfile.WorkingRegions = _context.AdminRegions.Where(item => item.Adminid == admin.Adminid).ToList();
                adminProfile.State = admin.Regionid;
            }

            return adminProfile;
        }
        #endregion

        #region ResetAdminPassword
        public void ResetAdminPassword(string email, string newPassword)
        {
            var account = _context.AspnetUsers.FirstOrDefault(item => item.Email == email);

            if (account != null)
            {
                string passwordhash = BC.HashPassword(newPassword);
                account.Passwordhash = passwordhash;
                _context.AspnetUsers.Update(account);
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Account not found");
            }
        }
        #endregion

        #region UpdateAdministrationInfo
        public void UpdateAdministrationInfo(string sessionEmail, string email, string mobileNo, string[] adminRegionIds)
        {
            var admin = GetAdminByEmail(sessionEmail);
            var aspnetUser = GetAspNetUserByEmail(sessionEmail);

            if (admin != null && aspnetUser != null)
            {
                admin.Email = email;
                admin.Mobile = mobileNo;
                _context.Admins.Update(admin);

                aspnetUser.Email = email;
                _context.AspnetUsers.Update(aspnetUser);

                var existingRegions = _context.AdminRegions.Where(item => item.Adminid == admin.Adminid).ToList();
                _context.AdminRegions.RemoveRange(existingRegions);

                foreach (string regionId in adminRegionIds)
                {
                    if (int.TryParse(regionId, out int regionIdInt))
                    {
                        _context.AdminRegions.Add(new AdminRegion { Adminid = admin.Adminid, Regionid = regionIdInt });
                    }
                }

                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Account not found");
            }
        }
        #endregion

        #region UpdateAccountingInfo
        public void UpdateAccountingInfo(string sessionEmail, string address1, string address2, string city, string zipcode, int state, string mobileNo)
        {
            var admin = GetAdminByEmail(sessionEmail);

            if (admin != null)
            {
                admin.Address1 = address1;
                admin.Address2 = address2;
                admin.City = city;
                admin.Zip = zipcode;
                admin.Regionid = state;
                admin.Mobile = mobileNo;

                _context.Admins.Update(admin);
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Account not found");
            }
        }
        #endregion

        #region GetProvider
        public List<ProviderVM> GetProviders(string region)
        {
            var providers = (from phy in _context.Physicians
                             join role in _context.Roles on phy.Roleid equals role.Roleid
                             join notify in _context.PhysicianNotifications on phy.Physicianid equals notify.Physicianid
                             where (string.IsNullOrEmpty(region) || phy.Regionid == int.Parse(region))
                             select new ProviderVM
                             {
                                 Name = phy.Firstname,
                                 status = phy.Status,
                                 Role = role.Name,
                                 OnCallStaus = new BitArray(new[] { notify.Isnotificationstopped[0] }),
                                 regions = _context.Regions.ToList(),
                                 physicianid = phy.Physicianid
                             }).ToList();
            return providers;
        }
        #endregion

        #region GetPhysicianProfile
        public ProviderProfileVm GetPhysicianProfile(int id)
        {
            var physician = _context.Physicians.FirstOrDefault(item => item.Physicianid == id);

            if (physician == null)
            {
                throw new InvalidOperationException("Physician not found");
            }

            var providerProfile = new ProviderProfileVm
            {
                FirstName = physician.Firstname,
                LastName = physician.Lastname ?? "",
                Email = physician.Email,
                Address1 = physician.Address1 ?? "",
                Address2 = physician.Address2 ?? "",
                City = physician.City ?? "",
                ZipCode = physician.Zip ?? "",
                MobileNo = physician.Mobile ?? "",
                Regions = _context.Regions.ToList(),
                MedicalLicense = physician.Medicallicense,
                NPINumber = physician.Npinumber,
                SynchronizationEmail = physician.Syncemailaddress,
                physicianid = physician.Physicianid,
                WorkingRegions = _context.PhysicianRegions.Where(item => item.Physicianid == physician.Physicianid).ToList(),
                State = physician.Regionid,
                SignatureFilename = physician.Signature,
                BusinessWebsite = physician.Businesswebsite,
                BusinessName = physician.Businessname,
                PhotoFileName = physician.Photo,
                IsAgreement = physician.Isagreementdoc,
                IsBackground = physician.Isbackgrounddoc,
                IsHippa = physician.Istrainingdoc,
                NonDiscoluser = physician.Isnondisclosuredoc,
                License = physician.Islicensedoc
            };

            return providerProfile;
        }
        #endregion

        #region ResetPhysicianPassword
        public bool ResetPhysicianPassword(int physicianId, string newPassword)
        {
            var physician = _context.Physicians.FirstOrDefault(item => item.Physicianid == physicianId);
            var account = GetAspNetUserByEmail(physician.Email);

            if (account != null && BC.Verify(newPassword, account.Passwordhash))
            {
                return false; // Password remains the same
            }
            else if (account != null)
            {
                string passwordHash = BC.HashPassword(newPassword);
                account.Passwordhash = passwordHash;
                _context.AspnetUsers.Update(account);
                _context.SaveChanges();
                return true; // Password updated successfully
            }
            else
            {
                return false; // Account not found
            }
        }
        #endregion

        #region UpdatePhysicianInformation
        public void UpdatePhysicianInformation(int id, string email, string mobileNo, string[] adminRegion, string synchronizationEmail, string npinumber, string medicalLicense)
        {
            var physician = _context.Physicians.FirstOrDefault(item => item.Physicianid == id);

            if (physician != null)
            {
                physician.Email = email;
                physician.Mobile = mobileNo;
                physician.Npinumber = npinumber;
                physician.Syncemailaddress = synchronizationEmail;
                physician.Medicallicense = medicalLicense;

                _context.Physicians.Update(physician);
                _context.SaveChanges();

                // Remove existing regions
                var existingRegions = _context.PhysicianRegions.Where(item => item.Physicianid == physician.Physicianid).ToList();
                _context.PhysicianRegions.RemoveRange(existingRegions);

                // Add new regions
                foreach (string regionValue in adminRegion)
                {
                    int regionId = int.Parse(regionValue); // Assuming region values are integer IDs
                    _context.PhysicianRegions.Add(new PhysicianRegion { Physicianid = physician.Physicianid, Regionid = regionId });
                }
                _context.SaveChanges(); // Save changes to add new associations
            }
            else
            {
                throw new InvalidOperationException("Physician not found");
            }
        }
        #endregion

        #region UpdateProviderProfile
        public void UpdateProviderProfile(int id, string businessName, string businessWebsite, IFormFile signatureFile, IFormFile photoFile)
        {
            var physician = _context.Physicians.FirstOrDefault(item => item.Physicianid == id);

            if (physician != null)
            {
                physician.Businessname = businessName;
                physician.Businesswebsite = businessWebsite;

                if (signatureFile != null && signatureFile.FileName != null)
                {
                    string signatureFileName = _uploadprovider.UploadSignature(signatureFile, id);
                    physician.Signature = signatureFileName;
                }

                if (photoFile != null && photoFile.FileName != null)
                {
                    string photoFileName = _uploadprovider.UploadPhoto(photoFile, id);
                    physician.Photo = photoFileName;
                }

                _context.Physicians.Update(physician);
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Physician not found");
            }
        }
        #endregion
    }
}
