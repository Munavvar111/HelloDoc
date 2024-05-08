using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.CustomModel.InvoiceVM;

namespace BusinessLayer.InterFace
{
    public interface IInvoiceInterface
    {
        public bool isFinalizeTimesheet(int physicianId, DateOnly startDate);
        public List<TimesheetDetail>? PostTimesheetDetails(int physicianId, DateOnly startDate, int afterDays, string aspNetUserId);
        public ViewTimeSheet GetTimesheetDetails(List<TimesheetDetail> td, List<TimesheetDetailReimbursement> tr, int physicianId);
        public List<TimesheetDetailReimbursement>? GetTimesheetBills(List<TimesheetDetail> TimeSheetDetails);

        public bool PutTimesheetDetails(List<ViewTimeSheetDetails> tds, string aspNetUserId);

        public bool TimeSheetBillAddEdit(TimeSheetDetailReimbursements trb, string AdminId);
         string UploadTimesheetDoc(IFormFile UploadFile, int TimeSheetId);
        public bool TimeSheetBillRemove(TimeSheetDetailReimbursements trb, string AdminId);
        public bool SetToFinalize(int timesheetid, string AdminId);


    }
}
