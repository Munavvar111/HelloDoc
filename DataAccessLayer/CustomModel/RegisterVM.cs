using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class RegisterVM
    {
        
        public string? Email { get; set; } = null!;

        [Key]
        [StringLength(20, ErrorMessage = "Password should be between {2} and {1} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]

        public string Passwordhash { get; set; } = null!;


        [DataType(DataType.Password)]
        [Compare("Passwordhash", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPasswordhash { get; set; }=null!;
    }
}
