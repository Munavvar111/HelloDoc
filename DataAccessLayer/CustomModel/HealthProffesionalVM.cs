using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class HealthProffesionalVM
    {
        
            [Required(ErrorMessage = "Business name is required.")]
            [Display(Name = "Business Name")]
            public string Vendorname { get; set; }

		public int VendorId { get; set; }


		[Required(ErrorMessage = "Profession is required.")]
            [Display(Name = "Select Business")]
            public int Profession { get; set; }

            [Required(ErrorMessage = "Fax number is required.")]
            [Display(Name = "Fax Number")]
            public string Faxnumber { get; set; }

            [Required(ErrorMessage = "Phone number is required.")]
            [Display(Name = "Phone Number")]
            [Phone(ErrorMessage = "Invalid phone number format.")]
            public string Phonenumber { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [Display(Name = "Email")]
            [EmailAddress(ErrorMessage = "Invalid email address format.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Business contact is required.")]
            [Display(Name = "Business Contact")]
            public string Businesscontact { get; set; }

            [Required(ErrorMessage = "Address is required.")]
            [Display(Name = "Street")]
            public string Address { get; set; }

            [Required(ErrorMessage = "City is required.")]
            [Display(Name = "City")]
            public string City { get; set; }

            [Required(ErrorMessage = "State is required.")]
            [Display(Name = "State")]
            public string State { get; set; }

            [Required(ErrorMessage = "Zip/Postal is required.")]
            [Display(Name = "Zip/Postal")]
            [RegularExpression(@"^\d{6}(-\d{4})?$", ErrorMessage = "Invalid Zip/Postal format.")]
            public string Zip { get; set; }
        
              public int HealthProfessionId { get; set; }
      }
}
