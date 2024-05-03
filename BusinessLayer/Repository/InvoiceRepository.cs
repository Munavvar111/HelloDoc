using BusinessLayer.InterFace;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using static DataAccessLayer.CustomModel.InvoiceVM;

namespace BusinessLayer.Repository
{
    public class InvoiceRepository : IInvoiceInterface
    {
        private readonly ApplicationDbContext _context;


        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;

        }
        public bool isFinalizeTimesheet(int physicianId, DateOnly startDate)
        {
            if (startDate == DateOnly.MinValue)
            {
                return true;
            }
            else
            {
                var data = _context.Timesheets.Where(e => e.PhysicianId == physicianId && e.StartDate == startDate).FirstOrDefault();
                if (data == null)
                {
                    return false;
                }
                else if (data.IsFinalize == false)
                {
                    return false;
                }
                return true;
            }
        }
        public List<TimesheetDetail>? PostTimesheetDetails(int physicianId, DateOnly startDate, int afterDays, string aspNetUserId)
        {
            var Timesheet = new Timesheet();

            var data = _context.Timesheets.Where(e => e.PhysicianId == physicianId && e.StartDate == startDate).FirstOrDefault();
            if (data == null && afterDays != 0)
            {
                DateOnly EndDate = startDate.AddDays(afterDays - 1);
                Timesheet.StartDate = startDate;
                Timesheet.PhysicianId = physicianId;
                Timesheet.IsFinalize = false;
                Timesheet.EndDate = EndDate;
                Timesheet.CreatedDate = DateTime.Now;
                Timesheet.CreatedBy = aspNetUserId;
                _context.Timesheets.Add(Timesheet);
                _context.SaveChanges();

                for (DateOnly i = startDate; i <= EndDate; i = i.AddDays(1))
                {
                    var timesheetdetail = new TimesheetDetail();
                    timesheetdetail.TimesheetId = Timesheet.TimesheetId;
                    timesheetdetail.TimesheetDate = i;
                    timesheetdetail.NumberOfHouseCall = 0;
                    timesheetdetail.NumberOfPhoneCall = 0;
                    timesheetdetail.TotalHours = 0;
                    _context.TimesheetDetails.Add(timesheetdetail);
                    _context.SaveChanges();
                }

                return _context.TimesheetDetails.Where(e => e.TimesheetId == Timesheet.TimesheetId).OrderBy(r => r.TimesheetDate).ToList();
            }
            else if (data == null && afterDays == 0)
            {
                return null;
            }
            else
            {
                return _context.TimesheetDetails.Where(e => e.TimesheetId == data.TimesheetId).OrderBy(r => r.TimesheetDate).ToList();
            }

        }
        public int FindOnCallProvider(int physicianId, DateOnly timeSheetDate)
        {
            int i = 0;
            var s = _context.Shifts.Where(r => r.Physicianid == physicianId).ToList();
            foreach (var item in s)
            {
                i += _context.Shiftdetails.Where(r => r.Shiftid == item.Shiftid && DateOnly.FromDateTime(r.Shiftdate) == timeSheetDate && r.Isdeleted == false).Count();
            }
            return i;
        }
        public ViewTimeSheet GetTimesheetDetails(List<TimesheetDetail> td, List<TimesheetDetailReimbursement> tr, int physicianId)
        {
            try
            {
                var TimeSheet = new ViewTimeSheet();

                TimeSheet.TimeSheetDetails = td.Select(e => new ViewTimeSheetDetails
                {
                    IsWeekend = e.IsWeekend == null || e.IsWeekend == false ? false : true,
                    NumberOfHouseCall = e.NumberOfHouseCall,
                    NumberOfPhoneCall = e.NumberOfPhoneCall,
                    OnCallhours = FindOnCallProvider(physicianId, e.TimesheetDate),
                    TimeSheetDate = e.TimesheetDate,
                    TimeSheetDetailId = e.TimesheetDetailId,
                    TotalHours = e.TotalHours,
                    TimeSheetId = e.TimesheetId
                }).OrderBy(r => r.TimeSheetDate).ToList();

                TimeSheet.TimeSheetDetailReimbursements = tr.Select(e => new TimeSheetDetailReimbursements
                {
                    Amount = e.Amount,
                    Timesheetdetailreimbursementid = e.TimesheetDetailReimbursementId,
                    Isdeleted = e.IsDeleted,
                    Itemname = e.ItemName,
                    Bill = e.Bill,
                    Createddate = (DateTime)e.CreatedDate,
                    Timesheetdate = _context.TimesheetDetails.Where(r => r.TimesheetDetailId == e.TimesheetDetailId).FirstOrDefault().TimesheetDate,
                    Timesheetid = _context.TimesheetDetails.Where(r => r.TimesheetDetailId == e.TimesheetDetailId).FirstOrDefault().TimesheetId,
                    Modifiedby = e.ModifiedBy,
                    Timesheetdetailid = e.TimesheetDetailId,
                }).OrderBy(r => r.Timesheetdetailid).ToList();

                TimeSheet.PayrateWithProvider = _context.PayrateByProviders.Where(r => r.PhysicianId == physicianId).ToList();
                if (td.Count > 0)
                {
                    TimeSheet.TimeSheetId = TimeSheet.TimeSheetDetails[0].TimeSheetId;
                }
                TimeSheet.PhysicianId = physicianId;
                return TimeSheet;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public List<TimesheetDetailReimbursement>? GetTimesheetBills(List<TimesheetDetail> TimeSheetDetails)
        {
            try
            {
                List<TimesheetDetailReimbursement>? TimeSheetBills = _context.TimesheetDetailReimbursements
                                                                         .Where(e => TimeSheetDetails.Contains(e.TimesheetDetail) && e.IsDeleted == false)
                                                                         .OrderBy(r => r.TimesheetDetailId)
                                                                         .ToList();

                return TimeSheetBills;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
