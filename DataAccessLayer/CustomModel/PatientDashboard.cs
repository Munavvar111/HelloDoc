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
        public Request Request { get; set; } = null!;

        public Requestwisefile Requestwisefile { get; set; } = null!;

        public User? User { get; set; }
    }
}
