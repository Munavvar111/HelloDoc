using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.CustomModel;

namespace BusinessLayer.InterFace
{
    public interface IAdmin
    {   
        List<NewRequestTableVM> SearchPatients(string searchValue, string selectValue, string selectedFilter,int[] currentStatus);
        List<NewRequestTableVM> GetAllData();
        ViewCaseVM GetCaseById(int id);
        Task UpdateRequestClient(ViewCaseVM viewCaseVM, int id);
        List<ViewNotesVM> GetNotesForRequest(int requestid);
        Task<bool> AssignRequest(int regionId, int physician, string description, int requestId,int adminid);
        Task<bool> UpdateAdminNotes(int requestId, string adminNotes);
        Task<bool> CancelCase(int requestId, string notes, string cancelReason);

        public bool IsSendEmail(string toEmail, string subject, string body, List<string> filenames);
        bool BlockRequest(string blockReason, int requestId);
        bool SendOrders(SendOrderModel order);
        SendOrderModel GetSendOrder(int requestid);

        ViewEncounterForm GetEncounterForm(int requestid);
        void SaveOrUpdateEncounterForm(ViewEncounterForm viewEncounterForm, string requestid);

    }
}
