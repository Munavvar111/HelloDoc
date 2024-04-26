using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.CustomModel
{
	public partial class RequestModel
	{
		[Key]
		[Column("email")]
		[Required(ErrorMessage = "Please Enter Email")]
		[RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
		public string Email { get; set; } = null!;

		[Column("username")]
		public string? Username { get; set; }

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

		[Column("birthdata")]
		[Required(ErrorMessage = "Please Enter BirthDate")]
		public DateOnly BirthDate { get; set; }

		[Column("street")]
		[Required(ErrorMessage = "Please Enter Street")]
		[StringLength(100, ErrorMessage = "Street should be between {2} and {1} characters.", MinimumLength = 2)]
		[RegularExpression(@"^[^\s]+$", ErrorMessage = "Invalid Street")]
		public string Street { get; set; } = null!;

		[Column("city")]
		[Required(ErrorMessage = "Please Enter City")]
		[StringLength(50, ErrorMessage = "City should be between {2} and {1} characters.", MinimumLength = 2)]
		public string City { get; set; } = null!;

		[Column("state")]
		[Required(ErrorMessage = "Please Enter State")]
		public string State { get; set; } = null!;

		[Column("zipcode")]
		[Required(ErrorMessage = "Please Enter Zipcode")]
		[RegularExpression(@"^\d{6}$", ErrorMessage = "Enter a valid 6-digit zip code")]
		public string Zipcode { get; set; } = null!;



		[DataType(DataType.Password)]
		[StringLength(20, ErrorMessage = "Password should be between {2} and {1} characters.", MinimumLength = 6)]
		public string? Passwordhash { get; set; }

		[DataType(DataType.Password)]
		[Compare("Passwordhash", ErrorMessage = "Passwords do not match.")]
		public string? ConfirmPasswordhash { get; set; }
		public string? FileName { get; set; }

		[RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "Please enter valid phone number")]
		[Required(ErrorMessage = "Plese enter your Phone Number"), Display(Name = " ")]
		[StringLength(20, MinimumLength = 10, ErrorMessage = "Enter valid Mobile Number")]
		public string? PhoneNo { get; set; }

		[FromForm]
		public IFormFile? File { get; set; }

		public string? Notes { get; set; }

		public List<Region>? Regions { get; set; }
	}



}
