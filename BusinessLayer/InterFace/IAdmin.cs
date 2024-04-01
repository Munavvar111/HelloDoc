using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Http;

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
        public List<int> GetUserPermissions(string roleid);
        public Menu GetMenufromMenuid(string menuid);

        AdminProfileVm GetAdminProfile(string email);
        void ResetAdminPassword(string email, string newPassword);
        Physician GetPhysicianByEmail(string email);
        Admin GetAdminByEmail(string email);
        AspnetUser GetAspNetUserByEmail(string email);
        void UpdateAdministrationInfo(string sessionEmail, string email, string mobileNo, string[] adminRegionIds);
        void UpdateAccountingInfo(string sessionEmail, string address1, string address2, string city, string zipcode, int state, string mobileNo);

        List<ProviderVM> GetProviders(string region);   

        ProviderProfileVm GetPhysicianProfile(int id);
        bool ResetPhysicianPassword(int physicianId, string newPassword);
        void UpdatePhysicianInformation(int id, string email, string mobileNo, string[] adminRegion, string synchronizationEmail, string npinumber, string medicalLicense);
        void UpdateProviderProfile(int id, string businessName, string businessWebsite, IFormFile signatureFile, IFormFile photoFile);
        bool UpdatePhysicianAccountingInfo(int physicianId, string address1, string address2, string city, int state, string zipcode, string mobileNo);
        void SaveNotification(List<int> physicianIds, List<bool> checkboxStates);
		List<ScheduleModel> GetEvents(int region);


	}
}
