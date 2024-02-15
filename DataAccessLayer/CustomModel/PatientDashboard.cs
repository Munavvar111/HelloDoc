using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DataModels;

namespace DataAccessLayer.CustomModel
{
    public class PatientDashboard
    {
        public Request Request { get; set; }

        public Requestwisefile Requestwisefile { get; set; }    

        public User? User { get; set; }
    }
}
