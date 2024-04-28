using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Http;
using System.Collections;

namespace DataAccessLayer.CustomModel
{
    public class CreateProviderVM
    {
        [Column("firstname")]
        [Required(ErrorMessage = "Please Enter FirstName")]
        [StringLength(50, ErrorMessage = "First Name should be between {2} and {1} characters.", MinimumLength = 2)]
        [RegularExpression(@"^[^\s]+$", ErrorMessage = "Invalid Name")]
        public string Firstname { get; set; } = null!;

        [Column("lastname")]
        [Required(ErrorMessage = "Please Enter LastName")]
        [RegularExpression(@"^[^\s]+$", ErrorMessage = "Invalid LastName")]
        [StringLength(50, ErrorMessage = "Last Name should be between {2} and {1} characters.", MinimumLength = 2)]
        public string Lastname { get; set; } = null!;

        [Column("username")]
        [Required(ErrorMessage = "Please Enter UserName")]
        [RegularExpression(@"^[^\s]+$", ErrorMessage = "Invalid UserName")]
        [StringLength(50, ErrorMessage = "Last Name should be between {2} and {1} characters.", MinimumLength = 2)]
        public string UserName { get; set; } = null!;


        [Column("email")]
        [Required(ErrorMessage = "Please Enter Email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Password should be between {2} and {1} characters.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters long, contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        public string Passwordhash { get; set; } = null!;

        public string? MedicalLicense { get; set; }
        public string? NPINumber { get; set; }
        public string? SynchronizationEmail { get; set; }

        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "Please enter valid phone number")]
        [Required(ErrorMessage = "Plese enter your Phone Number"), Display(Name = " ")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Enter valid Mobile Number")]
        public string? PhoneNo { get; set; }

        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public int? State { get; set; }

        [Required(ErrorMessage ="Please Enter A Role")]
        public int RoleId { get; set; }
        public IFormFile? Photo { get; set; }
        public string? ZipCode { get; set; }
        public List<Region>? Regions { get; set; }
        public List<Role>? Roles { get; set; }

        [Required(ErrorMessage = "Please Enter A BusinessName")]
        public string BusinessName { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter A BusinessWebsite")]
        public string BusinessWebsite { get; set; } = null!;
        public string? AdminNotes { get; set; }


        [Required(ErrorMessage = "Plese Enter A Photo")]
        public IFormFile File { get; set; } = null!;
        public IFormFile? signature { get; set; }
        public IFormFile? IsAgreement { get; set; }
        public IFormFile? IsBackground { get; set; }
        public IFormFile? IsHippa { get; set; }
        public IFormFile? NonDiscoluser { get; set; }
        public IFormFile? License { get; set; }
    }
}
