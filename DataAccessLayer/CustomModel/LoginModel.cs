using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.CustomModel;

[Table("LoginModel")]
public partial class LoginModel
{
    
    [StringLength(256)]
    public string? Username { get; set; }
    public string Email { get; set; } = null!;   

    [Key]
    [StringLength(256)]
    public string Passwordhash { get; set; } = null!;

    public string? ConfirmPasswordhash { get; set; }
}
