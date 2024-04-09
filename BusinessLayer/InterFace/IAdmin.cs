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
        List<Region> GetAllRegion();
        ViewCaseVM GetCaseById(int id);
        Task UpdateRequestClient(ViewCaseVM viewCaseVM, int id);
        List<ViewNotesVM> GetNotesForRequest(int requestid);
        Task<bool> AssignRequest(int regionId, int physician, string description, int requestId,int adminid);
        Task<bool> UpdateAdminNotes(int requestId, string adminNotes,string AspNetId,bool IsPhysician);
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
        void CreateShift(ScheduleModel data, string email);
        List<DateTime> IsShiftOverwritting(ScheduleModel data);
        ProviderOnCallVM GetProvidersOnCall(int region);
        IQueryable<Region> GetRegionsByRegionId(int regionId);
        Shiftdetail GetShiftDetailById(int shiftDetailId);

        void UpdateShiftDetail(Shiftdetail shiftdetail);
        void UpdateHealthPrifessional(Healthprofessional healthprofessional);
        void SaveChanges();
        IEnumerable<object> GetReviewShift(int region);
        List<Healthprofessionaltype> GetAllHealthprofessoionalType();

        IEnumerable<SendOrderModel> PartnerFilter(int healthProType, string vendorname);
        Healthprofessional GetHealthprofessionalById(int healthprofessionalId);
        List<User> GetUsers(string firstName, string lastName, string email, string phoneNumber);
        List<PatientHistoryVM> GetPatientRecords(int userId);
        void CreatePartner(HealthProffesionalVM healthProffesionalVM);
        Region GetRegionByName(string state);
        IEnumerable<Physician> GetPhysiciansByRegion(int region);

        List<Role> GetAllRoles();
        Role GetAllRolesById(int roleId);

        void AddRoles(Role role);

        void RemoveRangeRoleMenu(List<Rolemenu> rolemenu);
        void UpdateRoles(Role role);    
        void AddRoleMenus(Rolemenu rolemenu);
        void UpdateRoleMenus(Rolemenu rolemenu);
        List<Menu> GetMenuByAccountType(int accounttype);
        List<int> GetRoleMenuIdByRoleId(int roleid);
        List<Rolemenu> GetRoleMenuById(int roleid);

        List<UserAccess> GetUserData(int role);
        List<Physician> GetPhysiciansByRegion(string region);

        void AddRequestStatusLog(Requeststatuslog requeststatuslog);
        Request GetRequestById(int requestId);

        void UpdateRequestStatusLog(Requeststatuslog updaterequeststatuslog);
        Requestclient GetRequestClientById(int requestid);
        void UpdateRequest(Request request);

        void AddRequestWiseFile(Requestwisefile requestwisefile);
        void UpdateRequestWiseFile(Requestwisefile requestwisefile);
	}
}
