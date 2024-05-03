using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class InvoiceVM
    {
        public class ViewTimeSheetDetails
        {
            public int TimeSheetDetailId { get; set; }
            public int TimeSheetId { get; set; }
            public DateOnly TimeSheetDate { get; set; }
            public int? OnCallhours { get; set; }
            public decimal? TotalHours { get; set; }
            public bool IsWeekend { get; set; }
            public int? NumberOfHouseCall { get; set; }
            public int? NumberOfPhoneCall { get; set; }
        }

        public class ViewTimeSheet
        {
            public List<ViewTimeSheetDetails> TimeSheetDetails { get; set; }
            public List<TimeSheetDetailReimbursements> TimeSheetDetailReimbursements { get; set; }
            public List<PayrateByProvider> PayrateWithProvider { get; set; }
            public int TimeSheetId { get; set; }
            public string? Bonus { get; set; }
            public string? AdminNotes { get; set; }
            public int PhysicianId { get; set; }
        }

        public class TimeSheetDetailReimbursements
        {
            public int? Timesheetdetailreimbursementid { get; set; } = null!;

            public int Timesheetdetailid { get; set; }
            public int Timesheetid { get; set; }

            public string Itemname { get; set; } = null!;

            public int? Amount { get; set; } = null!;
            public DateOnly Timesheetdate { get; set; }
            public string Bill { get; set; } = null!;
            public IFormFile BillFile { get; set; }

            public bool? Isdeleted { get; set; }

            public string Createdby { get; set; } = null!;

            public DateTime Createddate { get; set; }

            public string? Modifiedby { get; set; }
        }
    }
}
