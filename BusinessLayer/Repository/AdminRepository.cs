using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;

namespace BusinessLayer.Repository
{
    public class AdminRepository:IAdmin
    {
        private readonly ApplicationDbContext _context;
        public AdminRepository(ApplicationDbContext context) { 
            _context = context;
        }

        public List<NewRequestTableVM> SearchPatients(string searchValue, string selectValue, string selectedFilter, int[] currentStatus)
        {
            var filteredPatients = (from req in _context.Requests
                                    join reqclient in _context.Requestclients
                                    on req.Requestid equals reqclient.Requestid
                                    select new NewRequestTableVM
                                    {
                                        PatientName = reqclient.Firstname,
                                        Requestor = req.Firstname + " " + req.Lastname,
                                        DateOfBirth = new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                                        ReqDate = req.Createddate,
                                        Phone = reqclient.Phonenumber,
                                        PhoneOther=req.Phonenumber,
                                        Address = reqclient.City + reqclient.Zipcode,
                                        Notes = reqclient.Notes,
                                        ReqTypeId = req.Requesttypeid,
                                        Email = req.Email,
                                        Id = reqclient.Requestclientid,
                                        regionid = reqclient.Regionid,
                                        Status = req.Status
                                    })
                                    .Where(item =>
                                        (string.IsNullOrEmpty(searchValue) || item.PatientName.Contains(searchValue)) &&
                                        (string.IsNullOrEmpty(selectValue) || item.regionid == int.Parse(selectValue)) &&
                                        (string.IsNullOrEmpty(selectedFilter) || item.ReqTypeId == int.Parse(selectedFilter)) &&
                                        currentStatus.Any(status=>item.Status==status)).ToList();

            return filteredPatients;
        }
    }
}
