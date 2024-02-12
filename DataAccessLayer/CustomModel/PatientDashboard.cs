using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class PatientDashboard
    {
        public DateTime CreatedDate {  get; set; }

        public int Status { get; set; } 

        public string Filename { get; set; }

    }
}
