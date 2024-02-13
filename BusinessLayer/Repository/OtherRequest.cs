using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Npgsql.PostgresTypes.PostgresCompositeType;

namespace BusinessLayer.Repository
{
    public class OtherRequest : IOtherRequest
    {
        private readonly ApplicationDbContext _context;
        public OtherRequest(ApplicationDbContext context) {
        _context  = context;
        }
        public Request GetRequestByEmail(string email)
        {
            return _context.Requests.FirstOrDefault(r => r.Email == email);
        }
        

        public void AddRequest(RequestOthers requestOthers, int ReqTypeId)
        {
            Request friend = new Request();
            friend.Requesttypeid = 2;//Friend 
            friend.Firstname = requestOthers.FirstNameOther;
            friend.Lastname = requestOthers.LastNameOther;
            friend.Email = requestOthers.EmailOther;
            friend.Status = 1;//Unsigned
            friend.Relationname = requestOthers.Relation;
            friend.Createddate = DateTime.Now;
            _context.Requests.Add(friend);
            _context.SaveChanges();
        }

        public void AddRequestClient(RequestOthers requestOthers, int RequestID)
        {
            Requestclient requestclient = new Requestclient();
            requestclient.Requestid = RequestID;
            requestclient.Firstname = requestOthers.FirstName;
            requestclient.Lastname = requestOthers.LastName;
            requestclient.Email = requestOthers.Email;
            requestclient.Intdate = requestOthers.BirthDate.Day;
            requestclient.Intyear = requestOthers.BirthDate.Year;
            requestclient.Strmonth = requestOthers.BirthDate.Month.ToString();
            requestclient.Street = requestOthers.Street;
            requestclient.City = requestOthers.City;
            requestclient.State = requestOthers.State;
            requestclient.Zipcode = requestOthers.Zipcode;
            _context.Requestclients.Add(requestclient);
            _context.SaveChanges();
        }
       public void AddFriendRequest(RequestOthers requestOthers, int RequestID)
        {
            AddRequest(requestOthers, RequestID);
            var request1=GetRequestByEmail(requestOthers.EmailOther); 
            AddRequestClient(requestOthers, request1.Requestid);
        }

        public void AddConceirgeRequest(RequestOthers addconciegeRequest, int RequestID)
        {
            AddRequest(addconciegeRequest, RequestID);
            Conceirge(addconciegeRequest);
            var request1 = GetRequestByEmail(addconciegeRequest.EmailOther);
            AddRequestClient(addconciegeRequest, request1.Requestid);

        }

        public void Conceirge(RequestOthers conceirge)
        {
            Concierge concierge = new Concierge();
            concierge.Conciergename = conceirge.FirstNameOther;
            concierge.City = conceirge.City;
            concierge.State = conceirge.State;
            concierge.Street = conceirge.Street;
            concierge.Zipcode = conceirge.Zipcode;
            concierge.Createddate = DateTime.Now;
            concierge.Regionid = 1;

            _context.Concierges.Add(concierge);
            _context.SaveChanges();
            var request1 = GetRequestByEmail(conceirge.EmailOther);
            ConceirgeRequest(request1.Requestid, concierge.Conciergeid);

        }

        public void ConceirgeRequest(int RequestID,int ConceirgeID)
        {
            Requestconcierge requestconcierge = new Requestconcierge();
            requestconcierge.Conciergeid = ConceirgeID;
            requestconcierge.Requestid = RequestID;

            _context.Requestconcierges.Add(requestconcierge);
            _context.SaveChanges();

        }
    }
}
