using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Please Enter Your Email")]
        public string Password { get; set; }

        public int? UserId { get; set; }

        public string? Token { get; set; }
    }
}
