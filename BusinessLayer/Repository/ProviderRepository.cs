using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Repository
{
    public class ProviderRepository : IProvider
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        public ProviderRepository(ApplicationDbContext context, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }


        #region SearchPatients
        public List<NewRequestTableVM> SearchPatients(string searchValue, string selectedFilter, int[] currentStatus, string Email)
        {
            Physician? physcian = _context.Physicians.Where(item => item.Email == Email).FirstOrDefault();
            List<NewRequestTableVM> filteredPatients = (from req in _context.Requests
                                                        join reqclient in _context.Requestclients
                                                        on req.Requestid equals reqclient.Requestid
                                                        join p in _context.Physicians
                                                        on req.Physicianid equals p.Physicianid into phy
                                                        from ps in phy.DefaultIfEmpty()
                                                        join encounter in _context.Encounterforms
                                                        on req.Requestid equals encounter.RequestId into enco
                                                        from eno in enco.Where(e => e != null).DefaultIfEmpty()
                                                        where req.Isdeleted == false
                                                        select new NewRequestTableVM
                                                        {
                                                            PatientName = reqclient.Firstname.ToLower(),
                                                            Requestor = req.Firstname + " " + req.Lastname,
                                                            DateOfBirth = reqclient != null && reqclient.Intyear != null && reqclient.Strmonth != null && reqclient.Intdate != null
                                                                            ? new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate)
                                                                            : new DateOnly(),
                                                            ReqDate = req.Createddate,
                                                            FirstName = reqclient.Firstname,
                                                            LastName = reqclient.Lastname,
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
                                                            PhysicianId = req.Physicianid,
                                                            PhysicianName = ps.Firstname + "_" + ps.Lastname,
                                                            CallType = (int)req.Calltype,
                                                            Cancel = _context.Casetags.Select(cc => new CancelCase
                                                            {
                                                                CancelCaseReson = cc.Name,
                                                                CancelReasonId = cc.Casetagid
                                                            }).ToList()
                                                        })
                                    .Where(item =>
                                        (string.IsNullOrEmpty(searchValue) || item.PatientName.Contains(searchValue)) &&
                                        (string.IsNullOrEmpty(selectedFilter) || item.ReqTypeId == int.Parse(selectedFilter)) &&
                                        currentStatus.Any(status => item.Status == status) && item.PhysicianId == physcian.Physicianid).ToList();
            return filteredPatients;
        }
        #endregion

        #region RequestAcceptedByProvider
        public bool RequestAcceptedByProvider(int requestId)
        {
            Request? request = _context.Requests.Find(requestId);
            if (request == null)
            {
                return false;
            }
            else
            {
                request.Status = 2;
                request.Accepteddate = DateTime.Now;
                _context.Requests.Update(request);
                _context.SaveChanges();
                return true;
            }
        }
        #endregion

    }
}
