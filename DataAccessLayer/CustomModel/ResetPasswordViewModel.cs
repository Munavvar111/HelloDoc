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
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters long, contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Please Confirm Your Password")]
		[Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Confirm Password must be at least 8 characters long, contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
		public string ConfirmPassword { get; set; }

		public string? UserId { get; set; }

        public string? Token { get; set; }
    }
}
