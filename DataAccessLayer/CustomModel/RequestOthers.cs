using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.CustomModel;

public partial class RequestOthers
{
    [Key]
    [StringLength(50, ErrorMessage = "First Name should be between {2} and {1} characters.", MinimumLength = 2)]
    [Required(ErrorMessage = "Please Enter First Name")]
    [RegularExpression(@"^(?!\s+$).+", ErrorMessage = "Enter a valid Name")]
    public string FirstNameOther { get; set; } = null!;

    [StringLength(50, ErrorMessage = "Last Name should be between {2} and {1} characters.", MinimumLength = 2)]
    [Required(ErrorMessage ="Please Enter Last Name")]
    [RegularExpression(@"^(?!\s+$).+", ErrorMessage = "Enter a valid Name")]
    public string LastNameOther { get; set; } = null!;

    [StringLength(256)]
    [Required(ErrorMessage = "Please Enter  Email")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
    public string EmailOther { get; set; } = null!;

    [StringLength(256)]
    public string? Relation { get; set; }

    [StringLength(256)]
    public string? BusinessName { get; set; }

    [StringLength(256)]
    public string? HotelName { get; set; }

    [StringLength(20, MinimumLength = 10, ErrorMessage = "Enter valid Mobile Number")]
    [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "Please enter valid phone number")]
    [Required(ErrorMessage = "Plese enter your Phone Number")]
    public string? PhoneNumber { get; set; }

    [StringLength(20, MinimumLength = 10, ErrorMessage = "Enter valid Mobile Number")]
    [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "Please enter valid phone number")]
    [Required(ErrorMessage = "Plese enter your Phone Number")]
    public string? PhoneNumberOther { get; set; }

    [StringLength(50, ErrorMessage = "First Name should be between {2} and {1} characters.", MinimumLength = 2)]
    [Required(ErrorMessage ="Please Enter FirstName Of Patient")]
    [RegularExpression(@"^(?!\s+$).+", ErrorMessage = "Enter a valid Name")]
    public string FirstName { get; set; }= null!;

    [StringLength(50, ErrorMessage = "Last Name should be between {2} and {1} characters.", MinimumLength = 2)]
    [Required(ErrorMessage ="Please Enter LastName Of Patient")]
    [RegularExpression(@"^(?!\s+$).+", ErrorMessage = "Enter a valid Name")]
    public string LastName { get; set; }=   null!;

    [StringLength(256)]
    [Required(ErrorMessage ="Please Enter The Email Of Patient")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
    public string Email { get; set; }=  null!;

    [Required(ErrorMessage ="Please Enter The DOB Of the Patient")]
    public DateOnly BirthDate { get; set; }

    [Required(ErrorMessage = "Please Enter Street")]
    [RegularExpression(@"^(?=.*\S)[a-zA-Z0-9\s.,'-]+$", ErrorMessage = "Enter a valid street address")]
    public string Street { get; set; } = null!;

    [Required(ErrorMessage = "Please Enter City")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Enter valid City")]
    [RegularExpression(@"^(?=.*\S)[a-zA-Z\s.'-]+$", ErrorMessage = "Enter a valid city name")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "State is required")]
    public string State { get; set; } = null!;

    [Required(ErrorMessage = "Zip Code is required")]
    [StringLength(10, ErrorMessage = "Enter valid Zip Code")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter a valid 5-digit zip code")]
    public string Zipcode { get; set; } = null!;

    [FromForm]
    public IFormFile? File { get; set; }

    public string? Notes { get; set; }

    public List<Region>? Regions { get; set; }

}
