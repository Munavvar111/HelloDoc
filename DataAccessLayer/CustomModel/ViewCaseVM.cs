using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "Please Enter Email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
        public string EmailView { get; set; }

        [Required(ErrorMessage ="Please Enter A Phone Number")]
        public string Phone { get; set; }
        public List<CancelCase>? Cancel { get; set; }


    }
    
}
