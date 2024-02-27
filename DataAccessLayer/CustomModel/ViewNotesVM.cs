using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class ViewNotesVM
    {

        public string[]? TransferedNotes { get; set; }

        public String? AdminNotes { get; set; }  

        public String? PhysicianNotes { get; set; }  

        public String AdditionalNotes { get; set; } 

        public int RequestId { get; set; }  
    }
}
