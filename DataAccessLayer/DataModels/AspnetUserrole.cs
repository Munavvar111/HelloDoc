using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DataModels;

[PrimaryKey("Userid", "Roleid")]
[Table("aspnet_userroles")]
public partial class AspnetUserrole
{
    [Key]
    [Column("userid")]
    [StringLength(128)]
    public string Userid { get; set; } = null!;

    [Key]
    [Column("roleid")]
    public int Roleid { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("AspnetUserroles")]
    public virtual AspnetUser User { get; set; } = null!;
}
