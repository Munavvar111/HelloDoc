using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class PatientHistoryVM
    {
		public string FirstName { get; set; }
		public string? ClientName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Address { get; set; }

		public int? UserId { get; set; }	
		public string? ConfirmationNumber { get; set; }	
		public DateTime? CreatedDate { get; set; }	
		public DateTime? ConcludeDate { get; set; }	
		public string? ProvideName { get; set; }
		public int? Status { get; set; }
		public int? RequestId { get; set; }
		public int? RequestClientId { get; set; }
		public bool? IsFinalize { get; set; }
	}
}
