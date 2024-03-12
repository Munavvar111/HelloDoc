using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class CloseCaseVM
    {
        public List<Requestwisefile>? Requestwisefileview { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateOnly BirthDate { get; set; }

        public string PhoneNo { get; set; } 

        public string Email { get; set; }
        public string ConfirmNumber { get; set; }       
    }
}
