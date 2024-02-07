using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.DataModels;

[PrimaryKey("Username", "Passwordhash")]
[Table("LoginModel")]
public partial class LoginModel
{
    [Key]
    [StringLength(256)]
    public string Username { get; set; } = null!;

    [Key]
    [StringLength(256)]
    public string Passwordhash { get; set; } = null!;
}
