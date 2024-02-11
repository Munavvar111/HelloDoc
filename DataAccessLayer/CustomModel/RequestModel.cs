using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public partial class RequestModel
    {
        [Key]
        [Column("email")]
        [Required(ErrorMessage ="Please Enter Email")]
        public string Email { get; set; } = null!;

        [Column("username")]
        public string? Username { get; set; }

        [Column("firstname")]
        [Required(ErrorMessage = "Please Enter FirstName")]
        public string Firstname { get; set; } = null!;

        [Column("lastname")]
        [Required(ErrorMessage = "Please Enter LastName")]
        public string Lastname { get; set; } = null!;
        
        [Column("birthdata")]
        [Required(ErrorMessage = "Please Enter BirthDate")]
        public DateOnly BirthDate { get; set; } 

        [Column("street")]
        [Required(ErrorMessage = "Please Enter Mobile")]
        public string Street { get; set; } = null!;

        [Column("city")]
        [Required(ErrorMessage = "Please Enter Mobile")]
        public string City { get; set; } = null!;

        [Column("state")]
        [Required(ErrorMessage = "Please Enter Mobile")]
        public string State { get; set; } = null!;

        [Column("zipcode")]
        [Required(ErrorMessage = "Please Enter Mobile")]
        public string Zipcode { get; set; } = null!;

    }
}
