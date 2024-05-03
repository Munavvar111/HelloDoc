using DataAccessLayer.DataModels;
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
    }
}
