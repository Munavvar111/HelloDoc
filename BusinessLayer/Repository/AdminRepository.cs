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
                                        Status = req.Status,
                                        RequestClientId=reqclient.Requestclientid,
                                        RequestId = reqclient.Requestid,
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
    }
}
