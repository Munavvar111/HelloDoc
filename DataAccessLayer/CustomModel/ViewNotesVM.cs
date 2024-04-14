using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class ViewNotesVM
    {
        public int? TransToPhysicianId { get; set; }
        public int? Status { get; set; }

        public DateTime? CreatedDate {  get; set; }  
        public string? AdminName { get; set; }
        public string? PhysicianName { get; set; }
        public string? AdminNotes { get; set; }
        public string? PhysicianNotes { get; set; }
        public string? TransferNotes { get; set; }
        public int? Cancelcount { get; set; }
    }

}
