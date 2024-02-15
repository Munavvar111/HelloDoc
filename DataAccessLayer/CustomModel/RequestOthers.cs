using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.CustomModel;

public partial class RequestOthers
{
    [Key]
    [StringLength(256)]
    [Required(ErrorMessage = "Please Enter First Name")]
    public string FirstNameOther { get; set; } = null!;

    [StringLength(256)]
    [Required(ErrorMessage ="Please Enter Last Name")]
    public string LastNameOther { get; set; } = null!;

    [StringLength(256)]
    [Required(ErrorMessage = "Please Enter  Email")]
    public string EmailOther { get; set; } = null!;

    [StringLength(256)]
    public string? Relation { get; set; }

    [StringLength(256)]
    public string? BusinessName { get; set; }

    [StringLength(256)]
    public string? HotelName { get; set; } 


    [StringLength(256)]
    [Required(ErrorMessage ="Please Enter FirstName Of Patient")]
    public string FirstName { get; set; }= null!;

    [StringLength(256)]
    [Required(ErrorMessage ="Please Enter LastName Of Patient")]
    public string LastName { get; set; }=   null!;

    [StringLength(256)]
    [Required(ErrorMessage ="Please Enter The Email Of Patient")]
    public string Email { get; set; }=  null!;

    [Required(ErrorMessage ="Please Enter The DOB Of the Patient")]
    public DateOnly BirthDate { get; set; }

    [Required(ErrorMessage = "Please Enter Street")]
    public string Street { get; set; } = null!;

    [Required(ErrorMessage = "Please Enter City")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "Please Enter State")]
    public string State { get; set; } = null!;

    [Required(ErrorMessage = "Please Enter Zipcode")]
    public string Zipcode { get; set; } = null!;

    [FromForm]
    public IFormFile? File { get; set; }

}
