using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class NewRequestTableVM
    {

        public string PhysicianName { get; set; }   
        public string PatientName {  get; set; }

        public int RequestClientId { get; set; }    

        public DateOnly DateOfBirth { get; set; }

        public string FirstName { get; set; }

        public string Location { get; set; }

        public string LastName { get; set; }   

        public string Requestor { get; set; }   

        public DateTime ReqDate { get; set; } 

        public string Address { get; set; } 

        public string Notes { get; set; }   

       public string Phone { get; set; }

        public Int32 ReqTypeId { get; set; }  

        public string Email { get; set; }   

        public Int32 Id { get; set; }

        public int? regionid {  get; set; }

        public int Status { get; set; }  

        public string PhoneOther { get; set; }

        public int RequestId { get;set; }   
    }
}
