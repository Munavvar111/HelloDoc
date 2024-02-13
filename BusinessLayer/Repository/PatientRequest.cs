using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BC = BCrypt.Net.BCrypt;

using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessLayer.Repository
{
    public class PatientRequest : IPatientRequest
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PatientRequest> _logger; // Inject the logger

        public PatientRequest(ApplicationDbContext context, ILogger<PatientRequest> logger) {
        _context = context;
            _logger = logger;
        }
       
        public AspnetUser GetUserByUserName(string userName)
        {
            return _context.AspnetUsers.FirstOrDefault(x => x.Username == userName);
        }
        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(x=>x.Email==email);
         
        }
        public Request GetRequestByEmail(string email)
        {
            return _context.Requests.OrderBy(e=>e.Requestid).LastOrDefault(r => r.Email == email);
        }

        public AspnetUser GetAspnetUserBYEmail(string email)
        {
            return _context.AspnetUsers.FirstOrDefault(r => r.Email == email);
        }
        public void AddAspnetUser(RequestModel requestModel)
        {
            var aspnetUser = new AspnetUser();
            {
                

                aspnetUser.Passwordhash = BC.HashPassword(requestModel.Passwordhash);

                // populate aspnetUser properties from requestModel
                aspnetUser.Aspnetuserid = Guid.NewGuid().ToString();
                aspnetUser.Email = requestModel.Email;
                aspnetUser.Username = requestModel.Firstname+requestModel.Lastname;
                    _context.AspnetUsers.Add(aspnetUser);
                    _context.SaveChanges();
                }
               
            
            
                

        }

        public void AddUser(RequestModel requestModel, String AspnetUserID)
        {
            var aspnetuser=GetAspnetUserBYEmail(requestModel.Email);
            var user = new User();
            {
                user.Email = requestModel.Email;
                user.Aspnetuserid = aspnetuser.Aspnetuserid;
                user.Firstname = requestModel.Firstname;
                user.Lastname = requestModel.Lastname;
                user.Street = requestModel.Street;
                user.City = requestModel.City;
                user.State = requestModel.State;
                user.Zipcode = requestModel.Zipcode;
                user.Intyear = requestModel.BirthDate.Year;
                user.Intdate = requestModel.BirthDate.Day;
                user.Strmonth = requestModel.BirthDate.Month.ToString();
                user.Createdby = requestModel.Firstname + requestModel.Lastname;
                user.Createddate = DateTime.Now;
                // populate user properties from requestModel
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        

        public void AddPatientRequest(RequestModel requestModel,int ReqTypeId)
        {
            using (var transactionScope = new TransactionScope())
            {

                try
                {


                    var user = GetUserByEmail(requestModel.Email);

                    if (user != null)
                    {
                        AddRequest(requestModel, user.Userid, ReqTypeId);
                        var request = GetRequestByEmail(requestModel.Email);
                        AddRequestClient(requestModel, request.Requestid);
                    }
                    else
                    {
                        AddAspnetUser(requestModel);
                        var aspnetuserId1 = GetAspnetUserBYEmail(requestModel.Email);
                        AddUser(requestModel, aspnetuserId1.Aspnetuserid);
                        var user1= GetUserByEmail(requestModel.Email);
                        AddRequest(requestModel, user1.Userid, ReqTypeId);
                        var request = GetRequestByEmail(requestModel.Email);
                        AddRequestClient(requestModel, request.Requestid);

                    }
                    transactionScope.Complete(); // Commit the transaction

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing AddPatientRequest.");

                }
            }

        }
        public void AddRequest(RequestModel requestModel, int UserId, int reqTypeId)
        {
            

                var request =new Request();
            {

            request.Userid = UserId;
            request.Requesttypeid=reqTypeId;
            request.Firstname = requestModel.Firstname;
            request.Lastname = requestModel.Lastname;
            request.Email = requestModel.Email;
            request.Createddate = DateTime.Now;
            request.Status = 1;
            }
        _context.Requests.Add(request);
            _context.SaveChanges();
        }
        

        public void AddRequestClient(RequestModel requestModel,int RequestID)
        {
            var requestClient = new Requestclient();
            {
                requestClient.Requestid = RequestID;
                requestClient.Firstname = requestModel.Firstname;
                requestClient.Lastname = requestModel.Lastname;
                requestClient.State = requestModel.State;
                requestClient.Street = requestModel.Street;
                requestClient.City = requestModel.City;
                requestClient.Zipcode = requestModel.Zipcode;
                // populate requestClient properties from requestModel
            };

            _context.Requestclients.Add(requestClient);
            _context.SaveChanges();
        }
        public void AddRequestWiseFile(string Filename,int RequestID)
        {
            var requestWiseFile=new Requestwisefile();
            {
                requestWiseFile.Filename = Filename;
                requestWiseFile.Createddate = DateTime.Now;
                requestWiseFile.Requestid= RequestID;
            }
            _context.Requestwisefiles.Add(requestWiseFile);

            _context.SaveChanges();

        }




    }
}
