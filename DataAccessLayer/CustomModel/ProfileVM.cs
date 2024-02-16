using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class ProfileVM
    {
        [Required(ErrorMessage ="Please Enter The First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage ="Plese Enter The Last Name")]
        public string LastName { get; set; } = null!;

        [DataType(DataType.DateTime, ErrorMessage = "Please enter a valid date.")]
        public DateTime BirthDate { get; set; } 

        [Required(ErrorMessage = "Plese Enter The Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Plese Enter The Street")] public string Street { get; set; } = null!;

        [Required(ErrorMessage = "Plese Enter The City")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Plese Enter The State")]
        public string State { get; set; } = null!;

        [Required(ErrorMessage = "Plese Enter The Zipcode")]
        public string ZipCode { get; set; } = null!;

        public string? PhoneNO { get; set; }    
    }
}
