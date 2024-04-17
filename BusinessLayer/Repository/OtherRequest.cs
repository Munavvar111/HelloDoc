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
        
        

        public void AddRequest(RequestOthers requestOthers, int ReqTypeId)
        {
            var friend = new DataAccessLayer.DataModels.Request();
            friend.Requesttypeid = ReqTypeId;//Friend 
            friend.Firstname = requestOthers.FirstNameOther;
            friend.Lastname = requestOthers.LastNameOther;
            friend.Email = requestOthers.EmailOther;
            friend.Status = 1;//Unsigned
            friend.Relationname = requestOthers.Relation;
            friend.Phonenumber = requestOthers.PhoneNumberOther;
            friend.Createddate = DateTime.Now;
            friend.Isdeleted = false;
            _context.Requests.Add(friend);
            _context.SaveChanges();
        }
        public Request GetRequestByEmail(string email)
        {
            return _context.Requests.OrderBy(e => e.Requestid).LastOrDefault(r => r.Email == email);
        }
        public void AddRequestClient(RequestOthers requestOthers, int RequestID)
        {
            var statebyregionid = _context.Regions.Where(item => item.Name ==requestOthers.State).FirstOrDefault();
            Requestclient requestclient = new Requestclient();
            requestclient.Requestid = RequestID;
            requestclient.Notes=requestOthers.Notes;
            requestclient.Firstname = requestOthers.FirstName;
            requestclient.Lastname = requestOthers.LastName;
            requestclient.Phonenumber = requestOthers.PhoneNumber;
            requestclient.Email = requestOthers.Email;
            requestclient.Intdate = requestOthers.BirthDate.Day;
            requestclient.Intyear = requestOthers.BirthDate.Year;
            requestclient.Strmonth = requestOthers.BirthDate.Month.ToString();
            requestclient.Street = requestOthers.Street;
            requestclient.City = requestOthers.City;
            requestclient.State = requestOthers.State;
            requestclient.Regionid = statebyregionid.Regionid;
            requestclient.Zipcode = requestOthers.Zipcode;
            requestclient.Address=requestclient.Street+","+requestclient.City+","+requestclient.State+","+requestclient.Zipcode;
            _context.Requestclients.Add(requestclient);
            _context.SaveChanges();
        }
       public void AddFriendRequest(RequestOthers requestOthers, int RequestTypeID)
        {
            AddRequest(requestOthers, RequestTypeID);
            var request1=GetRequestByEmail(requestOthers.EmailOther); 
            AddRequestClient(requestOthers, request1.Requestid);
            var region = _context.Regions.Where(x => x.Name == requestOthers.State).FirstOrDefault();
            int count = _context.Requests.Where(x => x.Createddate.Date == request1.Createddate.Date).Count() + 1;
            if (region != null)
            {
                var confirmNum = string.Concat(region.Abbreviation.ToUpper(), request1.Createddate.ToString("ddMMyy"), requestOthers.LastName.Substring(0, 2).ToUpper() ?? "",
               requestOthers.FirstName.Substring(0, 2).ToUpper(), count.ToString("D4"));
                request1.Confirmationnumber = confirmNum;
            }
            else
            {
                var confirmNum = string.Concat("ML", request1.Createddate.ToString("ddMMyy"), requestOthers.LastName.Substring(0, 2).ToUpper() ?? "",
              requestOthers.FirstName.Substring(0, 2).ToUpper(), count.ToString("D4"));
                request1.Confirmationnumber = confirmNum;
            }
        }

        public void AddConceirgeRequest(RequestOthers addconciegeRequest, int RequestTypeID)
        {
            AddRequest(addconciegeRequest, RequestTypeID);
            Conceirge(addconciegeRequest);
            var request1 = GetRequestByEmail(addconciegeRequest.EmailOther);
            AddRequestClient(addconciegeRequest, request1.Requestid);
            var region = _context.Regions.Where(x => x.Name == addconciegeRequest.State).FirstOrDefault();
            int count = _context.Requests.Where(x => x.Createddate.Date == request1.Createddate.Date).Count() + 1;
            if (region != null)
            {
                var confirmNum = string.Concat(region.Abbreviation.ToUpper(), request1.Createddate.ToString("ddMMyy"), addconciegeRequest.LastName.Substring(0, 2).ToUpper() ?? "",
               addconciegeRequest.FirstName.Substring(0, 2).ToUpper(), count.ToString("D4"));
                request1.Confirmationnumber = confirmNum;
            }
            else
            {
                var confirmNum = string.Concat("ML", request1.Createddate.ToString("ddMMyy"), addconciegeRequest.LastName.Substring(0, 2).ToUpper() ?? "",
              addconciegeRequest.FirstName.Substring(0, 2).ToUpper(), count.ToString("D4"));
                request1.Confirmationnumber = confirmNum;
            }
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
