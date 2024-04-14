
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class ViewNotes
    {
        public int CancelAdmincount { get; set; }
        public int CancelPhysiciancount { get; set; }
        public int CancelPatientcount { get; set; }
        public List<ViewNotesVM> viewnotes { get; set; }
    }
}
