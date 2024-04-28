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
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq;

namespace BusinessLayer.Repository
{
    public class AdminRepository : IAdmin
    {
        private readonly ApplicationDbContext _context;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IUploadProvider _uploadprovider;

        [Obsolete]
        public AdminRepository(ApplicationDbContext context, IUploadProvider uploadProvider, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _context = context;

            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _uploadprovider = uploadProvider;
        }

        public List<Healthprofessionaltype> GetAllHealthprofessoionalType()
        {
            return _context.Healthprofessionaltypes.ToList();
        }
        public List<Role> GetAllRoles()
        {
            return _context.Roles.ToList();
        }
        public void AddRoles(Role role)
        {
            _context.Roles.Add(role);
        }
        public Role GetAllRolesById(int roleId)
        {

            Role? role = _context.Roles.Where(item => item.Roleid == roleId).FirstOrDefault();
            if (role == null)
            {
                throw new Exception("Role not found"); // or use a more specific exception type
            }
            return role;

        }
        public void AddRoleMenus(Rolemenu rolemenu)
        {
            _context.Rolemenus.Add(rolemenu);
        }
        public List<Menu> GetMenuByAccountType(int accounttype)
        {
            List<Menu> menus = _context.Menus.Where(item => accounttype == 0 || item.Accounttype == accounttype).ToList();
            return menus;
        }
        public List<Region> GetAllRegion()
        {
            return _context.Regions.ToList();
        }

        public List<Region> GetPhysicianWorkingRegion(int physicianId)
        {
            return _context.PhysicianRegions.Where(item => item.Physicianid == physicianId).Select(item => item.Region).ToList();
        }
        public Physician? GetPhysicianByEmail(string email)
        {
            return _context.Physicians.FirstOrDefault(item => item.Email == email);
        }
        public Admin? GetAdminByEmail(string email)
        {
            return _context.Admins.Include(r => r.Aspnetuser).FirstOrDefault(item => item.Email == email);
        }

        public Shiftdetail? GetShiftDetailById(int shiftDetailId)
        {
            return _context.Shiftdetails.Include(s => s.Shift).FirstOrDefault(item => item.Shiftdetailid == shiftDetailId);
        }
        public List<int> GetRoleMenuIdByRoleId(int roleId)
        {
            List<int> Rolemenus = _context.Rolemenus.Where(item => item.Roleid == roleId).Select(item => item.Menuid).ToList();
            return Rolemenus;
        }

        public Blockrequest? GetBlockrequestById(int id)
        {
            return _context.Blockrequests.Include(item => item.Request).FirstOrDefault(item => item.Blockrequestid == id && item.Isactive == true);

        }
        public Blockrequest? GetBlockRequestByEmail(string email)
        {
            return _context.Blockrequests.FirstOrDefault(_ => _.Email.Trim() == email.Trim() && _.Isactive.Value);
        }


        public bool RequestIdExists(int requestId)
        {
            return _context.Requests.Any(item => item.Requestid == requestId);
        }

        public List<Rolemenu> GetRoleMenuById(int roleId)
        {
            List<Rolemenu> Rolemenus = _context.Rolemenus.Where(item => item.Roleid == roleId).ToList();
            return Rolemenus;
        }
        public void UpdateRoleMenus(Rolemenu rolemenu)
        {
            _context.Rolemenus.Update(rolemenu);
        }

        public void UpdateRoles(Role role)
        {
            _context.Roles.Update(role);
        }
        public void RemoveRangeRoleMenu(List<Rolemenu> rolemenu)
        {
            _context.Rolemenus.RemoveRange(rolemenu);
        }

        public IQueryable<Region> GetRegionsByRegionId(int regionId)
        {
            return _context.Regions.Where(i => i.Regionid == regionId);
        }

        public AspnetUser GetAspNetUserByEmail(string email)
        {
            return _context.AspnetUsers.FirstOrDefault(item => item.Email == email);
        }

        public Admin? GetAdminEmailById(int adminId)
        {
            return _context.Admins
                      .Where(item => item.Adminid == adminId)
                      .FirstOrDefault();
        }

        public void UpdatePhysicianDataBase(Physician physician)
        {
            _context.Physicians.Update(physician);
        }
        public List<int> GetUserPermissions(string roleid)
        {
            var menulist = _context.Rolemenus.Where(item => item.Roleid == int.Parse(roleid)).Select(item => item.Menuid).ToList();
            return menulist;
        }

        public Healthprofessional? GetHealthprofessionalById(int healthprofessionalId)
        {
            return _context.Healthprofessionals.Find(healthprofessionalId);
        }
        public Menu? GetMenufromMenuid(string menuid)
        {
            var menu = _context.Menus.Where(item => item.Menuid == int.Parse(menuid)).FirstOrDefault();
            return menu;
        }
        public void UpdateShiftDetail(Shiftdetail shiftdetail)
        {
            _context.Shiftdetails.Update(shiftdetail);
        }

        public void UpdateRequestStatusLog(Requeststatuslog requeststatuslog)
        {
            _context.Requeststatuslogs.Update(requeststatuslog);
        }

        public List<Role> GetRoleFromAccountType(int accountType)
        {
            return _context.Roles.Where(item => item.Accounttype == accountType).ToList();
        }

        public void UpdateRequestClientDataBase(Requestclient requestclient)
        {
            _context.Requestclients.Update(requestclient);
        }

        public void AddAspNetUser(AspnetUser aspnetUser)
        {
            _context.AspnetUsers.Add(aspnetUser);
        }
        public void AddPhysician(Physician physician)
        {
            _context.Physicians.Add(physician);
        }
        public void AddAdmin(Admin admin)
        {
            _context.Admins.Add(admin);
        }

        public List<CancelCase> GetCancelCases()
        {
            return _context.Casetags
                .Select(cc => new CancelCase
                {
                    CancelCaseReson = cc.Name,
                    CancelReasonId = cc.Casetagid
                }).ToList();
        }

        public void UpdateHealthPrifessional(Healthprofessional healthprofessional)
        {
            _context.Healthprofessionals.Update(healthprofessional);
        }

        public void AddRequestStatusLog(Requeststatuslog requeststatuslog)
        {
            _context.Requeststatuslogs.Add(requeststatuslog);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public List<string> GetMenuNamesByRoleId(int roleId)
        {
            return _context.Rolemenus
                .Include(b => b.Menu)
                .Where(item => item.Roleid == roleId)
                .Select(item => item.Menu.Name)
                .ToList();
        }

        public Request? GetRequestById(int requestId)
        {
            return _context.Requests.Include(req => req.Requestclients).Where(item => item.Requestid == requestId).FirstOrDefault();
        }
        public Region? GetRegionByName(string state)
        {
            return _context.Regions.Where(r => r.Name == state).FirstOrDefault();
        }

        public Requestclient? GetRequestClientById(int requestid)
        {
            return _context.Requestclients.Include(b => b.Request).Where(item => item.Requestclientid == requestid).FirstOrDefault();
        }

        public Requestclient? GetRequestclientByRequestId(int requestId)
        {
            return _context.Requestclients.Include(b => b.Request).Where(item => item.Requestid == requestId).FirstOrDefault();
        }

        public Requestnote? GetRequestNotesByRequestId(int requestid)
        {
            return _context.Requestnotes.Where(item => item.Requestid == requestid).FirstOrDefault();
        }

        public Requestwisefile? GetRequestwisefileByFileName(string FileName)
        {
            return _context.Requestwisefiles.FirstOrDefault(item => item.Filename == FileName);
        }

        public void UpdateRequest(Request request)
        {
            _context.Requests.Update(request);
        }



        public void AddRequestWiseFile(Requestwisefile requestwisefile)
        {
            _context.Requestwisefiles.Add(requestwisefile);

        }
        public void UpdateRequestWiseFile(Requestwisefile requestwisefile)
        {
            _context.Requestwisefiles.Update(requestwisefile);
        }

        public void AddRequestNotes(Requestnote requestnote)
        {
            _context.Requestnotes.Add(requestnote);
        }

        public void UpdateRequestNotes(Requestnote requestnote)
        {
            _context.Requestnotes.Update(requestnote);
        }

        public void UpdateBlockRequest(Blockrequest blockrequest)
        {
            _context.Blockrequests.Update(blockrequest);
        }
        public Physician? GetPhysicianById(int physicianId)
        {
            Physician? physician = _context.Physicians.Include(b => b.Aspnetuser).Where(item => item.Physicianid == physicianId).FirstOrDefault();
            return physician;
        }

        public List<PhysicianLocation> GetAllPhysicianLocation()
        {
            return _context.PhysicianLocations.ToList();
        }

        public Encounterform? GetEncounteFormByRequestId(int requestid)
        {
            return _context.Encounterforms.Include(r => r.Request).FirstOrDefault(item => item.RequestId == requestid);
        }

        public void UpdateEncounterForm(Encounterform encounterform)
        {
            _context.Encounterforms.Update(encounterform);
        }

        public DateTime? GetDateofService(int requestid)
        {
            Requeststatuslog? log = _context.Requeststatuslogs.OrderByDescending(x => x.Createddate).FirstOrDefault(x => x.Requestid == requestid && x.Status == 6 && x.Physicianid != null);
            return log?.Createddate;
        }

        public DateTime? GetCloseDate(int requestid)
        {
            Requeststatuslog? log = _context.Requeststatuslogs.OrderByDescending(x => x.Createddate).FirstOrDefault(x => x.Requestid == requestid && x.Status == 9);
            return log?.Createddate;
        }
        public string? GetPatientCancellationNotes(int requestid)
        {
            Requeststatuslog? log = _context.Requeststatuslogs.OrderByDescending(x => x.Createddate).FirstOrDefault(x => x.Requestid == requestid && x.Status == 3 && x.Physicianid != null);
            return log?.Notes;
        }

        public List<Healthprofessional> GetHealthProfessionalByHealthProfessionalId(int healthprofessionalId)
        {
            return _context.Healthprofessionals.Where(item => item.Healthprofessionalid == healthprofessionalId).ToList();

        }

        public void AddAspnetUserRole(string UserId, int RoleId)
        {
            AspnetUserrole aspnetUserrole = new AspnetUserrole();
            aspnetUserrole.Userid = UserId;
            aspnetUserrole.Roleid = RoleId;
            _context.AspnetUserroles.Add(aspnetUserrole);
            _context.SaveChanges();
        }

        #region GetSearchRecords
        public List<SearchRecordVM> GetSearchRecords(string email, string phoenNo, string patientName, string providerName, int[] status, int requestTypeId, DateTime fromDos, DateTime toDos)
        {
            var data = (from r in _context.Requests
                        join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                        join p in _context.Physicians on r.Physicianid equals p.Physicianid into prJoin
                        from p in prJoin.DefaultIfEmpty()
                        join rn in _context.Requestnotes on r.Requestid equals rn.Requestid into rrnJoin
                        from rn in rrnJoin.DefaultIfEmpty()
                        select new
                        {
                            Request = r,
                            RequestClient = rc,
                            Physician = p,
                            RequestNote = rn
                        }).ToList();

            var searchRecords = data.Select(item => new SearchRecordVM
            {
                PatientName = $"{item.RequestClient.Firstname} {item.RequestClient.Lastname}",
                Requestor = $"{item.Request.Firstname} {item.Request.Lastname}",
                DateOfService = item.Request.Accepteddate,
                ServiceDate = GetDateofService(item.Request.Requestid)?.ToString("MMMM dd, yyyy") ?? "",
                DateofClose = GetCloseDate(item.Request.Requestid)?.ToString("MMMM dd, yyyy") ?? "",
                CloseDate = GetCloseDate(item.Request.Requestid),
                Email = item.RequestClient.Email,
                PhoneNumber = item.RequestClient.Phonenumber,
                Address = item.RequestClient.Location,
                Zip = item.RequestClient.Zipcode,
                RequestStatus = item.Request.Status,
                PhysicianName = item.Physician != null ? $"{item.Physician.Firstname} {item.Physician.Lastname}" : "", // Handle null Physician
                PhysicianNote = item.RequestNote?.Physiciannotes,
                CancelledByProvidor = GetPatientCancellationNotes(item.Request.Requestid),
                PatientNote = item.RequestClient.Notes,
                RequestTypeId = item.Request.Requesttypeid,
                AdminNotes = item.RequestNote?.Adminnotes,
                RequestId = item.Request.Requestid,
                IsDelted = item.Request.Isdeleted,
            }).ToList();
            var searchRecord = searchRecords.Where(item =>
      (string.IsNullOrEmpty(email) || item.Email.ToLower().Contains(email.ToLower())) &&
      (string.IsNullOrEmpty(phoenNo) || item.PhoneNumber.Contains(phoenNo)) &&
      (string.IsNullOrEmpty(patientName) || item.PatientName.ToLower().Trim().Contains(patientName)) &&
      (string.IsNullOrEmpty(providerName) || item.PhysicianName.ToLower().Contains(providerName)) &&
      (status.Length == 0 || status.Contains(item.RequestStatus)) && item.IsDelted == false &&
      (requestTypeId == 0 || item.RequestTypeId == requestTypeId) &&
      (fromDos == DateTime.MinValue || item.DateOfService?.Date >= fromDos.Date) &&
      (toDos == new DateTime() || item.DateOfService?.Date <= toDos.Date)).ToList();
            return searchRecord;
        }
        #endregion

        #region GetBlockRequests
        public List<Blockrequest> GetBlockRequests(string name, string email, string phoneNumber)
        {
            return _context.Blockrequests
                .Include(b => b.Request)
                .Where(item =>
                    (string.IsNullOrEmpty(name) || item.Request.Firstname.Contains(name)) &&
                    (string.IsNullOrEmpty(email) || item.Email == email) &&
                    (string.IsNullOrEmpty(phoneNumber) || item.Phonenumber == phoneNumber))
                .ToList();
        }
        #endregion

        #region ShiftExists
        public bool ShiftExists(DateTime startDate, TimeOnly startTime, TimeOnly endTime, Shiftdetail shiftdetail)
        {
            return _context.Shiftdetails.Any(sd =>
                                               sd.Shift.Physicianid == shiftdetail.Shift.Physicianid && sd.Isdeleted == false &&
                                               sd.Shiftdate.Equals(new DateTime(startDate.Year, startDate.Month, startDate.Day)) &&
                                               (
                                               (sd.Starttime.Hour <= startTime.Hour && startTime.Hour <= endTime.Hour) ||
                                               (startTime.Hour <= endTime.Hour && endTime.Hour <= endTime.Hour)
                                               )
                                               );
        }
        #endregion

        public RequestStatusCounts GetStatusCountsAsync(int id)
        {
            return new RequestStatusCounts
            {
                NewCount = _context.Requests
                    .Where(item => item.Status == 1 && item.Isdeleted == false && (id == 0 || item.Physicianid == id))
                    .Count(),
                PendingCount = _context.Requests
                    .Where(item => item.Status == 2 && item.Isdeleted == false && (id == 0 || item.Physicianid == id))
                    .Count(),
                ActiveCount = _context.Requests
                    .Where(item => (item.Status == 4 || item.Status == 5) && item.Isdeleted == false && (id == 0 || item.Physicianid == id))
                    .Count(),
                ToClosedCount = _context.Requests
                    .Where(item => (item.Status == 3 || item.Status == 7 || item.Status == 8) && item.Isdeleted == false && (id == 0 || item.Physicianid == id))
                    .Count(),
                ConcludeCount = _context.Requests
                    .Where(item => item.Status == 6 && item.Isdeleted == false && (id == 0 || item.Physicianid == id))
                    .Count(),
                UnpaidCount = _context.Requests
                    .Where(item => item.Status == 9 && item.Isdeleted == false && (id == 0 || item.Physicianid == id))
                    .Count()
            };
        }

        public void AddPhysicianRegion(int PhysicianId, int RegionId)
        {
            PhysicianRegion physicianRegion = new PhysicianRegion();
            physicianRegion.Physicianid = PhysicianId;
            physicianRegion.Regionid = RegionId;
            _context.PhysicianRegions.Add(physicianRegion);
            _context.SaveChanges();
        }
        public void AddAdminRegion(int AdminId, int RegionId)
        {
            AdminRegion adminRegion = new AdminRegion();
            adminRegion.Adminid = AdminId;
            adminRegion.Regionid = RegionId;
            _context.AdminRegions.Add(adminRegion);
            _context.SaveChanges();
        }

        public void AddPhysicianNotification(int PhysicianId)
        {
            PhysicianNotification physicianNotification = new PhysicianNotification();
            physicianNotification.Physicianid = PhysicianId;
            physicianNotification.Isnotificationstopped = new BitArray(new[] { true });
            _context.PhysicianNotifications.Add(physicianNotification);
            _context.SaveChanges();

        }

        #region UpdatePhysicianLocation
        public bool UpdatePhysicianLocation(decimal latitude, decimal longitude, int physicianId)
        {
            PhysicianLocation? physicianLocationById = _context.PhysicianLocations.Include(b => b.Physician).Where(item => item.Physicianid == physicianId).FirstOrDefault();

            if (physicianLocationById != null)
            {
                physicianLocationById.Longitude = longitude;
                physicianLocationById.Latitude = latitude;
                physicianLocationById.Physicianname = physicianLocationById.Physician.Firstname;

                _context.PhysicianLocations.Update(physicianLocationById);
                _context.SaveChanges();
            }
            else
            {
                PhysicianLocation physicianLocation = new PhysicianLocation();
                physicianLocation.Latitude = latitude;
                physicianLocation.Longitude = longitude;
                physicianLocation.Physicianid = physicianId;
                physicianLocation.Createddate = DateTime.Now;
                _context.PhysicianLocations.Add(physicianLocation);
                _context.SaveChanges();
            }
            return true;
        }
        #endregion

        #region SearchPatients
        public List<NewRequestTableVM> SearchPatients(string searchValue, string selectValue, string selectedFilter, int[] currentStatus)
        {

            var filteredPatients = _context.Requests.Include(b => b.Requestclients).Include(b => b.Physician)
                .Include(b => b.Encounterforms).Where(req => !req.Isdeleted.Value).Where(req => req.Encounterforms != null).Select(req => new
                {
                    req = req,
                    reqclient = req.Requestclients.FirstOrDefault(),
                    ps = req.Physician,
                    eno = req.Encounterforms.FirstOrDefault()
                }).ToList();


            var searchRecords = filteredPatients.Select(item => new NewRequestTableVM
            {
                PatientName = item.reqclient?.Firstname.ToLower() ?? "",
                Requestor = $"{item.req?.Firstname} {item.req?.Lastname}",
                DateOfBirth = item.reqclient != null && item.reqclient.Intyear != null && item.reqclient.Strmonth != null && item.reqclient.Intdate != null
                                    ? new DateOnly((int)item.reqclient.Intyear, int.Parse(item.reqclient.Strmonth), (int)item.reqclient.Intdate)
                                    : new DateOnly(),
                ReqDate = item.req?.Createddate ?? new DateTime(),
                FirstName = item.reqclient?.Firstname ?? "",
                LastName = item.reqclient?.Lastname ?? "",
                Phone = item.reqclient?.Phonenumber ?? "",
                PhoneOther = item.req?.Phonenumber ?? "",
                Address = $"{item.reqclient?.City} {item.reqclient?.Zipcode}",
                Notes = GetDashboardNotesName(item.req.Requestid),
                ReqTypeId = item.req?.Requesttypeid ?? 0,
                Email = item.req?.Email ?? "",
                Id = item.reqclient?.Requestclientid ?? 0,
                regionid = item.reqclient?.Regionid ?? 0,
                Status = item.req.Status,
                RequestedDate = item.req.Createddate.Date,
                DateOfService = GetDateOfService(item.req.Requestid),
                State = item.reqclient.State,
                RequestClientId = item.reqclient?.Requestclientid ?? 0,
                RequestId = item.reqclient?.Requestid ?? 0,
                isfinalize = item.eno?.IsFinalize ?? false,
                Regions = _context.Regions.ToList(),

                PhysicianName = $"{item.ps?.Firstname} {item.ps?.Lastname}",
                Cancel = _context.Casetags.Select(cc => new CancelCase
                {
                    CancelCaseReson = cc.Name,
                    CancelReasonId = cc.Casetagid
                }).ToList()
            }).ToList();

            var searchRecord = searchRecords.Where(item =>
                                    (string.IsNullOrEmpty(searchValue) || item.PatientName.ToLower().Contains(searchValue.ToLower().Trim())) &&
                                    (string.IsNullOrEmpty(selectValue) || item.regionid == int.Parse(selectValue)) &&
                                    (string.IsNullOrEmpty(selectedFilter) || item.ReqTypeId == int.Parse(selectedFilter)) &&
                                    currentStatus.Any(status => item.Status == status)).OrderBy(item => item.RequestedDate).ToList();

            return searchRecord;
        }

        #endregion

        #region GetAllDataTable
        public List<NewRequestTableVM> GetAllData()
        {

            IQueryable<NewRequestTableVM> GetAllData = from req in _context.Requests
                                                       join reqclient in _context.Requestclients
                                                       on req.Requestid equals reqclient.Requestid
                                                       join p in _context.Physicians on req.Physicianid equals p.Physicianid into phy
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
        public ViewCaseVM GetCaseById(int id, int accountId)
        {
            // Retrieve the request client with associated request
            var requestClientWithRequest = _context.Requestclients
                .Include(reqclient => reqclient.Request)
                .Where(reqclient => reqclient.Requestclientid == id)
                .FirstOrDefault();

            if (requestClientWithRequest != null)
            {
                // Access the associated request
                var req = requestClientWithRequest.Request;

                // Construct the ViewCaseVM object
                var viewCaseVM = new ViewCaseVM
                {
                    status = req.Status,
                    RequestId = req.Requestid,
                    Notes = requestClientWithRequest.Notes,
                    FirstName = requestClientWithRequest.Firstname,
                    LastName = requestClientWithRequest.Lastname ?? "",
                    DateOfBirth = requestClientWithRequest.Intyear != null && requestClientWithRequest.Strmonth != null && requestClientWithRequest.Intdate != null ? new DateOnly((int)requestClientWithRequest.Intyear, int.Parse(requestClientWithRequest.Strmonth), (int)requestClientWithRequest.Intdate) : new DateOnly(),
                    Phone = requestClientWithRequest.Phonenumber ?? "",
                    EmailView = requestClientWithRequest.Email ?? "",
                    Location = requestClientWithRequest.Location,
                    RegionId = requestClientWithRequest.Regionid,
                    RequestClientId = requestClientWithRequest.Requestclientid,
                    PhysicianId = req.Physicianid
                };

                return viewCaseVM;
            }
            else
            {
                // Return an empty ViewCaseVM if the request client is not found
                return new ViewCaseVM();
            }
        }

        #endregion

        #region UpdateViewCase
        public async Task UpdateRequestClient(ViewCaseVM viewCaseVM, int id)
        {
            Requestclient? requestclient = await _context.Requestclients.Include(b => b.Request).Where(item => item.Requestclientid == id).FirstOrDefaultAsync();

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
                requestclient.Regionid = viewCaseVM.RegionId;


                _context.Update(requestclient);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region GetNotes
        public ViewNotes GetNotesForRequest(int requestid)
        {
            IQueryable<ViewNotesVM> leftJoin = from rn in _context.Requestnotes
                                               join rs in _context.Requeststatuslogs on rn.Requestid equals rs.Requestid into rsJoin
                                               from rs in rsJoin.DefaultIfEmpty()
                                               join a in _context.Admins on rs.Adminid equals a.Adminid into aJoin
                                               from a in aJoin.DefaultIfEmpty()
                                               join p in _context.Physicians on rs.Transtophysicianid equals p.Physicianid into pJoin
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
                                                   CreatedDate = rs.Createddate,
                                                   Cancelcount = _context.Requeststatuslogs.Count(item => item.Status == 3 || item.Status == 7)
                                               };

            IQueryable<ViewNotesVM> rightJoin = from rs in _context.Requeststatuslogs
                                                join rn in _context.Requestnotes on rs.Requestid equals rn.Requestid into rnJoin
                                                from rn in rnJoin.DefaultIfEmpty()
                                                join a in _context.Admins on rs.Adminid equals a.Adminid into aJoin
                                                from a in aJoin.DefaultIfEmpty()
                                                join p in _context.Physicians on rs.Transtophysicianid equals p.Physicianid into pJoin
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
                                                    CreatedDate = rs.Createddate,
                                                    Cancelcount = _context.Requeststatuslogs.Count(item => item.Status == 3 || item.Status == 7)
                                                };
            List<ViewNotesVM> result = leftJoin.Union(rightJoin).ToList();
            if (result.Count > 0)
            {
                ViewNotes viewnotes = new ViewNotes()
                {
                    CancelAdmincount = _context.Requeststatuslogs.Where(item => item.Requestid == requestid && item.Status == 3 && item.Adminid != null).Count(),
                    CancelPhysiciancount = _context.Requeststatuslogs.Where(item => item.Requestid == requestid && item.Status == 3 && item.Physicianid != null).Count(),
                    CancelPatientcount = _context.Requeststatuslogs.Where(item => item.Requestid == requestid && item.Status == 7).Count(),
                    viewnotes = result,
                };
                return viewnotes;
            }
            else
            {
                ViewNotes viewnotes = new ViewNotes()
                {
                    CancelAdmincount = _context.Requeststatuslogs.Where(item => item.Requestid == requestid && item.Status == 3 && item.Adminid != null).Count(),
                    CancelPhysiciancount = _context.Requeststatuslogs.Where(item => item.Requestid == requestid && item.Status == 3 && item.Physicianid != null).Count(),
                    CancelPatientcount = _context.Requeststatuslogs.Where(item => item.Requestid == requestid && item.Status == 7).Count(),
                    viewnotes = result,
                };
                return viewnotes;
            }

        }
        #endregion

        #region AssignRequest
        public async Task<bool> AssignRequest(int regionId, int physician, string description, int requestId, int adminid)
        {
            try
            {
                Request? request = await _context.Requests.FindAsync(requestId);

                if (request != null)
                {
                    request.Status = 1;
                    request.Physicianid = physician;
                    request.Modifieddate = DateTime.Now;
                    _context.Requests.Update(request);

                    Requeststatuslog requestStatusLog = new Requeststatuslog
                    {
                        Adminid = adminid,
                        Notes = description,
                        Requestid = requestId,
                        Status = 1,
                        Createddate = DateTime.Now,
                        Transtophysicianid = physician,
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
        public async Task<bool> UpdateAdminNotes(int requestId, string adminNotes, string aspNetId, bool isPhysician)
        {
            try
            {
                Requestnote? requestVisenotes = await _context.Requestnotes.FirstOrDefaultAsync(item => item.Requestid == requestId);

                if (requestVisenotes != null)
                {
                    if (isPhysician)
                    {
                        requestVisenotes.Physiciannotes = adminNotes;
                    }
                    else
                    {
                        requestVisenotes.Adminnotes = adminNotes;
                    }
                    requestVisenotes.Modifiedby = aspNetId;
                    requestVisenotes.Modifieddate = DateTime.Now;
                    _context.Update(requestVisenotes);
                }
                else
                {
                    if (!isPhysician)
                    {
                        Requestnote requestNotes = new Requestnote
                        {
                            Requestid = requestId,
                            Adminnotes = adminNotes,
                            Createddate = DateTime.Now,
                            Createdby = aspNetId
                        };
                        _context.Add(requestNotes);
                    }
                    else
                    {
                        Requestnote requestNotes = new Requestnote
                        {
                            Requestid = requestId,
                            Physiciannotes = adminNotes,
                            Createddate = DateTime.Now,
                            Createdby = aspNetId
                        };
                        _context.Add(requestNotes);
                    }
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
        public async Task<bool> CancelCase(int requestId, string notes, string cancelReason, int adminid)
        {
            try
            {
                Request? requestById = await _context.Requests.FindAsync(requestId);

                if (requestById != null)
                {
                    requestById.Status = 3;
                    requestById.Casetag = _context.Casetags.FirstOrDefault(c => c.Casetagid == int.Parse(cancelReason)).Name;
                    requestById.Modifieddate = DateTime.Now;
                    _context.Requests.Update(requestById);

                    Requeststatuslog requestStatusLog = new Requeststatuslog
                    {
                        Status = 3,
                        Requestid = requestId,
                        Notes = notes,
                        Createddate = DateTime.Now,
                        Adminid = adminid
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
                        Requestid = requestId,
                        Createddate = DateTime.Now,
                        Reason = blockReason,
                        Isactive = true
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
            var admin = _context.Admins.Include(b => b.Aspnetuser).FirstOrDefault(item => item.Email == email);
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
                adminProfile.Username = admin.Aspnetuser.Username;
                adminProfile.RoleId = admin.Roleid;
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
                aspnetUser.Phonenumber = mobileNo;
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
                             join phyregion in _context.PhysicianRegions on phy.Physicianid equals phyregion.Physicianid
                             where ((string.IsNullOrEmpty(region) || phyregion.Regionid == int.Parse(region)) && (phy.Isdeleted == null || !phy.Isdeleted.Value))
                             select new ProviderVM
                             {
                                 Name = phy.Firstname,
                                 status = phy.Status,
                                 Role = role.Name,
                                 OnCallStaus = new BitArray(new[] { notify.Isnotificationstopped[0] }),
                                 IsNotificationStoped = notify.Isnotificationstopped,
                                 regions = _context.Regions.ToList(),
                                 physicianid = phy.Physicianid
                             }).Distinct().ToList();
            foreach (var provider in providers)
            {
                var onCall = GetProvidersOnCall(0, provider.physicianid);
                provider.OnCallStaus[0] = onCall.OnDuty.Any(item => item.Physicianid == provider.physicianid);
            }
            
            return providers;
        }
        #endregion

        #region GetPhysicianProfile
        public ProviderProfileVm GetPhysicianProfile(int id)
        {
            var physician = _context.Physicians.Include(b => b.Aspnetuser).FirstOrDefault(item => item.Physicianid == id);
            var role = _context.Roles.Where(item => item.Accounttype == 2).ToList();
            if (physician == null)
            {
                throw new InvalidOperationException("Physician not found");
            }

            var providerProfile = new ProviderProfileVm
            {
                Username = physician.Aspnetuser.Username ?? "",
                FirstName = physician.Firstname,
                LastName = physician.Lastname ?? "",
                Email = physician.Email,
                Status = physician.Status,
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
                License = physician.Islicensedoc,
                IsCredential = physician.Iscredentialdoc,
                Roles = role,
                PhysicianRole = (int)physician.Roleid,
            };

            return providerProfile;
        }
        #endregion

        #region ResetPhysicianPassword
        public bool ResetPhysicianPassword(int physicianId, string newPassword)
        {
            var physician = _context.Physicians.FirstOrDefault(item => item.Physicianid == physicianId);
            if (physician == null)
            {
                return false;
            }
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
        public void UpdatePhysicianInformation(int id, string email, string mobileNo, string[] adminRegion, string synchronizationEmail, string npinumber, string medicalLicense, string userId)
        {
            var physician = _context.Physicians.Include(req => req.Aspnetuser).FirstOrDefault(item => item.Physicianid == id);

            if (physician != null)
            {
                physician.Email = email;
                physician.Aspnetuser.Email = email;
                physician.Mobile = mobileNo;
                physician.Npinumber = npinumber;
                physician.Syncemailaddress = synchronizationEmail;
                physician.Medicallicense = medicalLicense;
                physician.Modifieddate = DateTime.Now;
                physician.Modifiedby = userId;
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
        public void UpdateProviderProfile(int id, string businessName, string businessWebsite, IFormFile signatureFile, IFormFile photoFile, string AspnetId)
        {
            var physician = _context.Physicians.FirstOrDefault(item => item.Physicianid == id);

            if (physician != null)
            {
                physician.Businessname = businessName;
                physician.Businesswebsite = businessWebsite;
                physician.Modifiedby = AspnetId;
                physician.Modifieddate = DateTime.Now;
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

        #region UpdatePhysicianAccountingInfo
        public bool UpdatePhysicianAccountingInfo(int physicianId, string address1, string address2, string city, int state, string zipcode, string mobileNo, string AspNetId)
        {
            Physician? physician = _context.Physicians.FirstOrDefault(item => item.Physicianid == physicianId);
            if (physician != null)
            {
                physician.Address1 = address1;
                physician.Address2 = address2;
                physician.City = city;
                physician.Regionid = state;
                physician.Zip = zipcode;
                physician.Mobile = mobileNo;
                physician.Modifieddate = DateTime.Now;
                physician.Modifiedby = AspNetId;
                _context.Physicians.Update(physician);
                _context.SaveChanges();
                return true; // Operation succeeded
            }
            else
            {
                return false; // Physician not found
            }
        }
        #endregion

        #region SaveNotification
        public void SaveNotification(List<int> physicianIds, List<bool> checkboxStates)
        {
            for (int i = 0; i < physicianIds.Count; i++)
            {
                int physicianId = physicianIds[i];
                bool isNotificationStopped = checkboxStates[i];

                PhysicianNotification? physicianNotification = _context.PhysicianNotifications
                    .FirstOrDefault(pn => pn.Physicianid == physicianId);

                if (physicianNotification != null)
                {
                    physicianNotification.Isnotificationstopped = new BitArray(new[] { isNotificationStopped });
                    _context.SaveChanges();
                }
            }

            // After updating the ones in the request, update the rest to false
            var allPhysicians = _context.PhysicianNotifications
                .Where(pn => !physicianIds.Contains(pn.Physicianid));

            foreach (var physician in allPhysicians)
            {
                physician.Isnotificationstopped = new BitArray(new[] { false });
            }
            _context.SaveChanges();
        }
        #endregion

        #region GetEventes
        public List<ScheduleModel> GetEvents(int region, bool IsPhysician, int id)
        {
            var eventswithoutdelet = (from s in _context.Shifts
                                      join pd in _context.Physicians on s.Physicianid equals pd.Physicianid
                                      join sd in _context.Shiftdetails on s.Shiftid equals sd.Shiftid into shiftGroup
                                      from sd in shiftGroup.DefaultIfEmpty()
                                      where !IsPhysician || (IsPhysician && pd.Physicianid == id)
                                      where (pd.Isdeleted == null || !pd.Isdeleted.Value)
                                      select new ScheduleModel
                                      {
                                          Shiftid = sd.Shiftdetailid,
                                          Status = sd.Status,
                                          Starttime = sd.Starttime,
                                          Endtime = sd.Endtime,
                                          Physicianid = s.Physicianid,
                                          PhysicianName = pd.Firstname + ' ' + pd.Lastname,
                                          Shiftdate = sd.Shiftdate,
                                          ShiftDetailId = sd.Shiftdetailid,
                                          Regionid = (int)sd.Regionid,
                                          ShiftDeleted = sd.Isdeleted
                                      }).Where(item => (region == 0 || item.Regionid == region)).ToList();
            var events = eventswithoutdelet.Where(item => !item.ShiftDeleted).ToList();
            return events;
        }
        #endregion

        #region CreateShift
        public void CreateShift(ScheduleModel data, string email)
        {
            Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
            List<DateTime> conflictingDates = new List<DateTime>(); // List to store conflicting dates
            Physician? physician = _context.Physicians.FirstOrDefault(item => item.Email == email);


            try
            {
                using (var transaction = new TransactionScope())
                {
                    Shift shift = new Shift();
                    shift.Physicianid = data.Physicianid;
                    shift.Repeatupto = data.Repeatupto;
                    shift.Startdate = data.Startdate;
                    if (admin != null)
                    {
                        shift.Createdby = admin.Aspnetuserid;
                    }
                    if (physician != null && physician.Aspnetuserid != null)
                    {
                        shift.Createdby = physician.Aspnetuserid;
                    }
                    shift.Createddate = DateTime.Now;
                    shift.Isrepeat = new BitArray(new[] { true });
                    shift.Repeatupto = data.Repeatupto;
                    _context.Shifts.Add(shift);
                    _context.SaveChanges();

                    Shiftdetail sd = new Shiftdetail();
                    sd.Shiftid = shift.Shiftid;
                    sd.Shiftdate = new DateTime(data.Startdate.Year, data.Startdate.Month, data.Startdate.Day);
                    sd.Starttime = data.Starttime;
                    sd.Endtime = data.Endtime;
                    sd.Regionid = data.Regionid;
                    sd.Status = data.Status;
                    sd.Isdeleted = false;
                    _context.Shiftdetails.Add(sd);
                    _context.SaveChanges();

                    Shiftdetailregion sr = new Shiftdetailregion();
                    sr.Shiftdetailid = sd.Shiftdetailid;
                    sr.Regionid = data.Regionid;
                    sr.Isdeleted = false;
                    _context.Shiftdetailregions.Add(sr);
                    _context.SaveChanges();

                    if (data.checkWeekday != null)
                    {
                        List<int> day = data.checkWeekday.Split(',').Select(int.Parse).ToList();

                        foreach (int d in day)
                        {
                            DayOfWeek desiredDayOfWeek = (DayOfWeek)d;
                            DateTime today = DateTime.Today;
                            DateTime nextOccurrence = new DateTime(data.Startdate.Year, data.Startdate.Month, data.Startdate.Day + 1);
                            int occurrencesFound = 0;
                            while (occurrencesFound < data.Repeatupto)
                            {
                                if (nextOccurrence.DayOfWeek == desiredDayOfWeek)
                                {

                                    Shiftdetail sdd = new Shiftdetail();
                                    sdd.Shiftid = shift.Shiftid;
                                    sdd.Shiftdate = nextOccurrence;
                                    sdd.Starttime = data.Starttime;
                                    sdd.Endtime = data.Endtime;
                                    sdd.Regionid = data.Regionid;
                                    sdd.Status = data.Status;
                                    sdd.Isdeleted = false;
                                    _context.Shiftdetails.Add(sdd);
                                    _context.SaveChanges();

                                    Shiftdetailregion srr = new Shiftdetailregion();
                                    srr.Shiftdetailid = sdd.Shiftdetailid;
                                    srr.Regionid = data.Regionid;
                                    srr.Isdeleted = false;
                                    _context.Shiftdetailregions.Add(srr);
                                    _context.SaveChanges();
                                    occurrencesFound++;
                                }
                                nextOccurrence = nextOccurrence.AddDays(1);
                            }
                        }
                    }

                    transaction.Complete();
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to submit Form", ex);
            }

        }
        #endregion

        #region GetProvidersOnCall
        public ProviderOnCallVM GetProvidersOnCall(int region, int physicianid)
        {
            var currentTime = DateTime.Now.Hour;
            var onDutyQuery = from shiftDetail in _context.Shiftdetails
                              join physician in _context.Physicians on shiftDetail.Shift.Physicianid equals physician.Physicianid
                              join physicianRegion in _context.PhysicianRegions on physician.Physicianid equals physicianRegion.Physicianid
                              where (region == 0 || physicianRegion.Regionid == region) &&
                              (physicianid == 0 || physician.Physicianid == physicianid) &&
                                    shiftDetail.Shiftdate.Date == DateTime.Now.Date &&
                                    currentTime >= shiftDetail.Starttime.Hour &&
                                    currentTime <= shiftDetail.Endtime.Hour &&
                                    !shiftDetail.Isdeleted && !physician.Isdeleted.Value
                              select physician;

            var onDuty = onDutyQuery.Distinct().ToList();

            var offDutyQuery = from physician in _context.Physicians
                               join physicianRegion in _context.PhysicianRegions on physician.Physicianid equals physicianRegion.Physicianid
                               where (region == 0 || physicianRegion.Regionid == region) &&
                                                             (physicianid == 0 || physician.Physicianid == physicianid) &&
                                     !_context.Shiftdetails.Any(item => item.Shift.Physicianid == physician.Physicianid &&
                                                                        item.Shiftdate.Date == DateTime.Now.Date &&
                                                                       currentTime >= item.Starttime.Hour &&
                                                                       currentTime <= item.Endtime.Hour &&
                                                                       !item.Isdeleted)
                               select physician;
            var offDuty = offDutyQuery.Distinct().ToList();

            return new ProviderOnCallVM
            {
                OnDuty = onDuty,
                OffDuty = offDuty
            };
        }
        #endregion

        #region IsShiftOverwritting
        public List<DateTime> IsShiftOverwritting(ScheduleModel data)
        {
            List<DateTime> OverlappingDates = new List<DateTime>();
            bool shiftExists = _context.Shiftdetails.Any(sd =>
                                    sd.Shift.Physicianid == data.Physicianid && sd.Isdeleted == false &&
                                    sd.Shiftdate.Equals(new DateTime(data.Startdate.Year, data.Startdate.Month, data.Startdate.Day)) &&
                                    (
                                    (sd.Starttime.Hour <= data.Starttime.Hour && data.Starttime.Hour <= sd.Endtime.Hour) ||
                                    (sd.Starttime.Hour <= data.Endtime.Hour && data.Endtime.Hour <= sd.Endtime.Hour)
                                    )
                                    );
            if (shiftExists)
            {
                OverlappingDates.Add(DateTime.Parse(data.Startdate.ToString()));
            }

            if (data.checkWeekday != null)
            {
                List<int> day = data.checkWeekday.Split(',').Select(int.Parse).ToList();

                foreach (int d in day)
                {
                    DayOfWeek desiredDayOfWeek = (DayOfWeek)d;
                    DateTime today = DateTime.Today;
                    DateTime nextOccurrence = new DateTime(data.Startdate.Year, data.Startdate.Month, data.Startdate.Day + 1);
                    int occurrencesFound = 0;
                    while (occurrencesFound < data.Repeatupto)
                    {
                        if (nextOccurrence.DayOfWeek == desiredDayOfWeek)
                        {
                            var shiftExistForRepeat = _context.Shiftdetails.Any(sd =>
                                    sd.Shift.Physicianid == data.Physicianid && sd.Isdeleted == false &&
                                    sd.Shiftdate.Equals(new DateTime(nextOccurrence.Year, nextOccurrence.Month, nextOccurrence.Day)) &&
                                    (
                                    (sd.Starttime.Hour <= data.Starttime.Hour && data.Starttime.Hour <= sd.Endtime.Hour) ||
                                    (sd.Starttime.Hour <= data.Endtime.Hour && data.Endtime.Hour <= sd.Endtime.Hour)
                                    )
                                    );
                            if (shiftExistForRepeat)
                            {
                                OverlappingDates.Add(DateTime.Parse(nextOccurrence.ToString()));
                            }
                            occurrencesFound++;
                        }
                        nextOccurrence = nextOccurrence.AddDays(1);
                    }
                }
            }
            return OverlappingDates;
        }
        #endregion

        #region GetReviewShift
        public IEnumerable<object> GetReviewShift(int region)
        {
            var shifts = (from shiftis in _context.Shifts
                          join shiftdetails in _context.Shiftdetails
                          on shiftis.Shiftid equals shiftdetails.Shiftid
                          join regionis in _context.Regions
                          on shiftdetails.Regionid equals regionis.Regionid
                          where !shiftdetails.Isdeleted
                          select new ReviewShiftVM
                          {
                              Shiftid = shiftis.Shiftid,
                              ShiftDetailId = shiftdetails.Shiftdetailid,
                              Firstname = shiftis.Physician.Firstname,
                              Lastname = shiftis.Physician.Lastname,
                              Shiftdate = shiftdetails.Shiftdate,
                              Starttime = shiftdetails.Starttime,
                              Endtime = shiftdetails.Endtime,
                              Regionid = (int)shiftdetails.Regionid,
                              RegionName = regionis.Name,
                              Status = shiftdetails.Status
                          }).Where(item => (region == 0 || item.Regionid == region) && item.Status == 1).ToList();
            return shifts;
        }
        #endregion

        #region PartnerFilter
        public IEnumerable<SendOrderModel> PartnerFilter(int healthProType, string vendorname)
        {
            var orders = (from helthprofesion in _context.Healthprofessionals
                          join helthprofessiontype in _context.Healthprofessionaltypes
                          on helthprofesion.Healthprofessionalid equals helthprofessiontype.Healthprofessionalid
                          where helthprofesion.Isdeleted == false
                          select new SendOrderModel
                          {
                              Professionname = helthprofessiontype.Professionname,
                              BusinessName = helthprofesion.Vendorname,
                              Email = helthprofesion.Email,
                              FaxNumber = helthprofesion.Faxnumber,
                              PhoneNumber = helthprofesion.Phonenumber,
                              BusinesContact = helthprofesion.Businesscontact,
                              VendorId = helthprofesion.Vendorid,
                              HelthProfessionId = (int)helthprofesion.Healthprofessionalid
                          })
                        .Where(item =>
                            (string.IsNullOrEmpty(vendorname) || item.BusinessName.ToLower().Contains(vendorname.ToLower().Trim())) &&
                            (healthProType == 0 || item.HelthProfessionId == healthProType))
                        .ToList();
            return orders;
        }
        #endregion

        #region GetUsers
        public List<User> GetUsers(string firstName, string lastName, string email, string phoneNumber)
        {
            return _context.Users.Where(item =>
                (string.IsNullOrEmpty(firstName) || item.Firstname.ToLower().Contains(firstName.ToLower().Trim())) &&
                (string.IsNullOrEmpty(lastName) || item.Lastname.ToLower().Contains(lastName.Trim().ToLower())) &&
                (string.IsNullOrEmpty(email) || item.Email.ToLower().Contains(email.Trim().ToLower())) &&
                (string.IsNullOrEmpty(phoneNumber) || item.Mobile.ToLower().Contains(phoneNumber.Trim().ToLower()))).ToList();
        }
        #endregion

        #region GetPatientRecords
        public List<PatientHistoryVM> GetPatientRecords(int userId)
        {
            return (from requestclient in _context.Requestclients
                    join encounterform in _context.Encounterforms
                    on requestclient.Requestid equals encounterform.RequestId into patientRecords
                    from totalPatient in patientRecords.DefaultIfEmpty()
                    where requestclient.Request.Userid == userId
                    select new PatientHistoryVM
                    {
                        ClientName = requestclient.Request.Firstname,
                        CreatedDate = requestclient.Request.Createddate,
                        ConfirmationNumber = requestclient.Request.Confirmationnumber,
                        ProvideName = requestclient.Request.Physician.Firstname,
                        Status = requestclient.Request.Status,
                        IsFinalize = totalPatient.IsFinalize,
                        RequestId = requestclient.Request.Requestid,
                        RequestClientId = requestclient.Requestclientid
                    }).ToList();
        }
        #endregion

        #region CreatePartner
        public void CreatePartner(HealthProffesionalVM healthProffesionalVM)
        {
            Healthprofessional healthprofessional = new Healthprofessional
            {
                Vendorname = healthProffesionalVM.Vendorname,
                Healthprofessionalid = healthProffesionalVM.Profession,
                Faxnumber = healthProffesionalVM.Faxnumber,
                Address = healthProffesionalVM.Address,
                City = healthProffesionalVM.City,
                Zip = healthProffesionalVM.Zip,
                Createddate = DateTime.Now,
                State = healthProffesionalVM.State,
                Businesscontact = healthProffesionalVM.Businesscontact,
                Phonenumber = healthProffesionalVM.Phonenumber,
                Email = healthProffesionalVM.Email,
                Isdeleted = false
            };

            var region = GetRegionByName(healthProffesionalVM.State);
            if (region != null)
                healthprofessional.Regionid = region.Regionid;

            _context.Healthprofessionals.Add(healthprofessional);
            _context.SaveChanges();
        }
        #endregion

        #region GetDateOfService
        public DateTime GetDateOfService(int requestid)
        {
            var requestStatusLog = _context.Requeststatuslogs
                ?.OrderByDescending(x => x.Createddate)
                ?.FirstOrDefault(x => x.Requestid == requestid && x.Status == 2);
            if (requestStatusLog == null)
            {
                return DateTime.MinValue;
            }
            return requestStatusLog.Createddate;
        }
        #endregion

        #region GetDashboardNotesName

        public string GetDashboardNotesName(int requestid)
        {
            Requeststatuslog? requeststatuslog = _context.Requeststatuslogs.OrderByDescending(x => x.Createddate).Where(x => x.Requestid == requestid).FirstOrDefault();
            Admin? admin = new Admin();
            Physician? physician = new Physician();
            Physician? transphysician = new Physician();

            if (requeststatuslog != null)
            {
                if (requeststatuslog.Adminid != null)
                {
                    admin = _context.Admins.FirstOrDefault(x => x.Adminid == requeststatuslog.Adminid);
                }
                if (requeststatuslog.Physicianid != null)
                {
                    physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == requeststatuslog.Physicianid);
                }
                if (requeststatuslog.Transtophysicianid != null)
                {
                    transphysician = _context.Physicians.FirstOrDefault(x => x.Physicianid == requeststatuslog.Transtophysicianid);
                }

                if (requeststatuslog.Adminid != null && admin != null && transphysician != null)
                {
                    if (requeststatuslog.Status == 1 && requeststatuslog.Transtophysicianid != null)
                    {
                        return "Admin " + admin.Firstname + " " + admin.Lastname + " transferred to Physician " + transphysician.Firstname + " " + transphysician.Lastname + " on " + requeststatuslog.Createddate.ToString();
                    }
                    else if (requeststatuslog.Status == 3)
                    {
                        return "Admin " + admin.Firstname + " " + admin.Lastname + " cancelled on " + requeststatuslog.Createddate.ToString();
                    }
                }

                if (requeststatuslog.Physicianid != null && physician != null)
                {
                    if (requeststatuslog.Status == 3)
                    {
                        return "Physician " + physician.Firstname + " " + physician.Lastname + " cancelled on " + requeststatuslog.Createddate.ToString();
                    }
                    else if (requeststatuslog.Status == 2 && requeststatuslog.Transtoadmin != new BitArray(new[] { true }))
                    {
                        if (requeststatuslog.Physicianid == requeststatuslog.Transtophysicianid)
                        {
                            return "Physician " + physician.Firstname + " " + physician.Lastname + " created request " + requeststatuslog.Createddate.ToString();
                        }
                        else
                        {
                            return "Physician " + physician.Firstname + " " + physician.Lastname + " accepted request on " + requeststatuslog.Createddate.ToString();
                        }
                    }
                    else if (requeststatuslog.Status == 2 && requeststatuslog.Transtoadmin[0])
                    {
                        return "Physician " + physician.Firstname + " " + physician.Lastname + " requested to transfer request on " + requeststatuslog.Createddate.ToString();
                    }
                    else if (requeststatuslog.Status == 8)
                    {
                        return "Physician " + physician.Firstname + " " + physician.Lastname + " concluded request " + requeststatuslog.Createddate.ToString();
                    }

                    return "";
                }

                if (requeststatuslog.Adminid == null && requeststatuslog.Physicianid == null)
                {
                    if (requeststatuslog.Status == 4)
                    {
                        return "Patient accepted agreement on " + requeststatuslog.Createddate.ToString();
                    }
                    else if (requeststatuslog.Status == 7)
                    {
                        return "Patient rejected agreement on " + requeststatuslog.Createddate.ToString();
                    }
                }
                if (requeststatuslog.Adminid != null)
                {
                    if (requeststatuslog.Status == 9)
                    {
                        return "Admin Closed The Request on" + requeststatuslog.Createddate.ToString();
                    }
                }
            }

            return "";
        }
        #endregion

        #region GetPhysiciansByRegion
        public IEnumerable<Physician> GetPhysiciansByRegion(int region)
        {
            var physicians = (from physicianRegion in _context.PhysicianRegions
                              where region == 0 || physicianRegion.Regionid == region
                              select physicianRegion.Physician)
                             .ToList();
            return physicians;
        }
        #endregion

        #region GetUserData
        public List<UserAccess> GetUserData(int role)
        {
            var list = (from aspuser in _context.AspnetUsers
                        join physician in _context.Physicians
                        on aspuser.Aspnetuserid equals physician.Aspnetuserid into physicians
                        from totalphy in physicians.DefaultIfEmpty()
                        join admin in _context.Admins
                        on aspuser.Aspnetuserid equals admin.Aspnetuserid into admins
                        from totaladmin in admins.DefaultIfEmpty()
                        join aspnetuserrole in _context.AspnetUserroles
                        on aspuser.Aspnetuserid equals aspnetuserrole.Userid into aspnetusersroles
                        from totalasprole in aspnetusersroles.DefaultIfEmpty()
                        join roletab in _context.Roles
                        on totalasprole.Roleid equals roletab.Roleid into rolesdata
                        from roles in rolesdata.DefaultIfEmpty()
                        where (role == 0 || roles.Accounttype == role)
                        where roles.Isdeleted != new BitArray(new[] { true })
                        where !totaladmin.Isdeleted || !totalphy.Isdeleted.Value
                        select (roles.Accounttype == 1 ?
                            new UserAccess
                            {
                                AccountType = roles.Name,
                                AccountPOC = totaladmin.Firstname,
                                phone = totaladmin.Mobile,
                                status = totaladmin.Adminid,
                                roleid = roles.Roleid,
                                AccountTypeid = roles.Accounttype,
                                OpenRequest = _context.Requests.Count(),
                                useraccessid = totaladmin.Adminid,
                            } : new UserAccess
                            {
                                AccountType = roles.Name,
                                AccountPOC = totalphy.Firstname,
                                phone = totalphy.Mobile,
                                status = totalphy.Status,
                                roleid = roles.Roleid,
                                OpenRequest = _context.Requests.Where(i => i.Physicianid == totalphy.Physicianid).Count(),
                                AccountTypeid = roles.Accounttype,
                                useraccessid = totalphy.Physicianid,
                            })).ToList();
            return list;
        }
        #endregion

        #region GetPhysician
        public List<Physician> GetPhysiciansByRegion(string region)
        {
            if (!int.TryParse(region, out int regionId))
            {
                throw new ArgumentException("Invalid region ID format.");
            }

            var physicians = (from physicianRegion in _context.PhysicianRegions
                              where physicianRegion.Regionid == regionId && (physicianRegion.Physician.Isdeleted == null || !physicianRegion.Physician.Isdeleted.Value)
                              select physicianRegion.Physician)
                             .ToList();

            return physicians;
        }
        #endregion


        #region GetEmailLogs
        public List<LogsVM> GetEmailLogs(int? accountType, string? receiverName, string? emailId, DateTime? createdDate, DateTime? sentDate)
        {
            var result = (from e in _context.Emaillogs.Include(req => req.Role)
                          select new LogsVM
                          {
                              isSms = false,
                              Recipient = e.Receivername,
                              ConfirmationNumber = e.Confirmationnumber,
                              Email = e.Emailid,
                              SentDate = e.Sentdate,
                              CreatedDate = e.Createdate,
                              Sent = e.Isemailsent,
                              SentTries = e.Senttries,
                              Action = e.Action,
                              RoleName = e.Role.Name,
                              AccountType = e.Role.Accounttype,
                          }).Where(item =>
                          (accountType == 0 || item.AccountType == accountType) &&
                          (string.IsNullOrEmpty(receiverName) || (item.Recipient != null && item.Recipient.ToLower().Contains(receiverName.Trim().ToLower()))) &&
                          (string.IsNullOrEmpty(emailId) || (item.Email != null && item.Email.ToLower().Contains(emailId.ToLower().Trim()))) &&
                          (createdDate == new DateTime() || item.CreatedDate.Value.Date == createdDate.Value.Date) &&
                          (sentDate == new DateTime() || item.SentDate.Value.Date == sentDate.Value.Date)).ToList();

            return result;
        }
        #endregion

        #region GetSmsLogs
        public List<LogsVM> GetSmsLogs(int? role, string? reciever, string? mobile, DateTime? createdDate, DateTime? sentDate)
        {
            var result = (from e in _context.Smslogs.Include(req => req.Role)
                          select new LogsVM
                          {
                              isSms = false,
                              Recipient = e.Receivername,
                              ConfirmationNumber = e.Confirmationnumber,
                              MobileNumber = e.Mobilenumber,
                              SentDate = e.Sentdate,
                              CreatedDate = e.Createdate,
                              Sent = e.Issmssent,
                              SentTries = e.Senttries,
                              Action = e.Action,
                              RoleName = e.Role.Name,
                              AccountType = e.Role.Accounttype
                          }).Where(item =>
                          (role == 0 || item.AccountType == role) &&
                          (string.IsNullOrEmpty(reciever) || (item.Recipient != null && item.Recipient.ToLower().Contains(reciever.Trim().ToLower()))) &&
                          (string.IsNullOrEmpty(mobile) || (item.Email != null && item.Email.ToLower().Contains(mobile.ToLower().Trim()))) &&
                          (createdDate == new DateTime() || item.CreatedDate.Value.Date == createdDate.Value.Date) &&
                          (sentDate == new DateTime() || item.SentDate.Value.Date == sentDate.Value.Date)).ToList();

            return result;
        }
        #endregion

    }
}

