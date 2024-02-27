using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class ViewCaseVM
    {
        public int? RequestClientId { get; set; }  
        public int? RequestId {  get; set; }

        public string? ConfirmationNumber { get; set; }  
        public DateOnly DateOfBirth { get; set; }
        public string FirstName { get; set; }

        public Int32 status { get; set; }
        public string? Location { get; set; }

        public string LastName { get; set; }
        public string? Notes { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }

    }
}
