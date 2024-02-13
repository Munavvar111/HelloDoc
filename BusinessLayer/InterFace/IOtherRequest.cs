using DataAccessLayer.CustomModel;
using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.InterFace
{
    public interface IOtherRequest
    {
        void AddRequest(RequestOthers requestOthers, int ReqTypeId);

        void AddRequestClient(RequestOthers requestOthers, int RequestID);

        Request GetRequestByEmail(string email);


        void AddFriendRequest(RequestOthers friendRequest,int ReqTypeId);

        void AddConceirgeRequest(RequestOthers addconciegeRequest,int ReqTypeId);

        void Conceirge(RequestOthers conceirge);

        void ConceirgeRequest(int RequestID,int ConceirgeID);
    }
}
