using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataModels;

namespace BusinessLayer.InterFace
{
    public interface IPatientRequest
    {

        User GetUserByEmail(string Email);

        Request GetRequestByEmail(string email);

        AspnetUser GetAspnetUserBYEmail(string Email);

        void AddAspnetUser(RequestModel requestModel);

        void AddUser(RequestModel requestModel, string AspnetUserID);

        void AddRequest(RequestModel requestModel,int UserId,int ReqTypeId);

        void AddRequestClient(RequestModel requestModel,int RequestID);
        
        void AddPatientRequest(RequestModel requestModel,int ReqTypeId);


    }
}
