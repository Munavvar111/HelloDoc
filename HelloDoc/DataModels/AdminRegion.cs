using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.DataModels;

[Table("admin_region")]
public partial class AdminRegion
{
    [Key]
    [Column("adminregionid")]
    public int Adminregionid { get; set; }

    [Column("adminid")]
    public int Adminid { get; set; }

    [Column("regionid")]
    public int Regionid { get; set; }

    [ForeignKey("Adminid")]
    [InverseProperty("AdminRegions")]
    public virtual Admin Admin { get; set; } = null!;

    [ForeignKey("Regionid")]
    [InverseProperty("AdminRegions")]
    public virtual Region Region { get; set; } = null!;
}
