using BusinessLayer.InterFace;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Http;
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

        public bool PutTimesheetDetails(List<ViewTimeSheetDetails> tds, string aspNetUserId)
        {
            try
            {
                foreach (var item in tds)
                {                                                                                                       
                    var td = _context.TimesheetDetails.Where(r => r.TimesheetDetailId == item.TimeSheetDetailId).FirstOrDefault();
                    td.TotalHours = item.TotalHours;
                    td.NumberOfHouseCall = item.NumberOfHouseCall;
                    td.NumberOfPhoneCall = item.NumberOfPhoneCall;
                    td.IsWeekend = item.IsWeekend;
                    td.ModifiedBy = aspNetUserId;
                    td.ModifiedDate = DateTime.Now;
                    _context.TimesheetDetails.Update(td);
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }


        public bool TimeSheetBillRemove(TimeSheetDetailReimbursements trb, string AdminId)
        {
            TimesheetDetailReimbursement data = _context.TimesheetDetailReimbursements.Where(e => e.TimesheetDetailReimbursementId == trb.Timesheetdetailreimbursementid).FirstOrDefault();
            if (data != null)
            {

                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = AdminId;
                data.IsDeleted = true;
                _context.TimesheetDetailReimbursements.Update(data);
                _context.SaveChanges();


                return true;
            }
            return false;

        }

        public  string UploadTimesheetDoc(IFormFile UploadFile, int TimeSheetId)
        {
            string newfilename = null;
            if (UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload\\TimeSheet\\" + TimeSheetId;
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}"; ;

                string fileNameWithPath = Path.Combine(path, newfilename);
                //upload_path = FilePath.Replace("wwwroot\\Upload\\TimeSheet\\", "/Upload/TimeSheet/") + "/" + newfilename;


                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }


            }

            return newfilename;
        }


        public bool TimeSheetBillAddEdit(TimeSheetDetailReimbursements trb, string AdminId)
        {
            TimesheetDetail data = _context.TimesheetDetails.Where(e => e.TimesheetDetailId == trb.Timesheetdetailid).FirstOrDefault();
            if (data != null && trb.Timesheetdetailreimbursementid == null)
            {
                TimesheetDetailReimbursement timesheetdetailreimbursement = new TimesheetDetailReimbursement();
                timesheetdetailreimbursement.TimesheetDetailId = trb.Timesheetdetailid;
                timesheetdetailreimbursement.Amount = (int)trb.Amount;
                timesheetdetailreimbursement.Bill = UploadTimesheetDoc(trb.BillFile, data.TimesheetId);
                timesheetdetailreimbursement.ItemName = trb.Itemname;
                timesheetdetailreimbursement.CreatedDate = DateTime.Now;
                timesheetdetailreimbursement.CreatedBy = AdminId;
                timesheetdetailreimbursement.IsDeleted = false;
                _context.TimesheetDetailReimbursements.Add(timesheetdetailreimbursement);
                _context.SaveChanges();


                return true;
            }
            else if (data != null && trb.Timesheetdetailreimbursementid != null)
            {
                TimesheetDetailReimbursement timesheetdetailreimbursement = _context.TimesheetDetailReimbursements.Where(r => r.TimesheetDetailReimbursementId == trb.Timesheetdetailreimbursementid).FirstOrDefault(); ;
                timesheetdetailreimbursement.Amount = (int)trb.Amount;

                timesheetdetailreimbursement.ItemName = trb.Itemname;
                timesheetdetailreimbursement.ModifiedDate = DateTime.Now;
                timesheetdetailreimbursement.ModifiedBy = AdminId;
                _context.TimesheetDetailReimbursements.Update(timesheetdetailreimbursement);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool SetToFinalize(int timesheetid, string AdminId)
        {
            try
            {
                var data = _context.Timesheets.Where(e => e.TimesheetId == timesheetid).FirstOrDefault();
                if (data != null)
                {
                    data.IsFinalize = true;
                    _context.Timesheets.Update(data);
                    _context.SaveChanges();
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }


    }
}
