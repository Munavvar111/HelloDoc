using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DataModels;

[Table("physician_region")]
public partial class PhysicianRegion
{
    [Key]
    [Column("physicianregionid")]
    public int Physicianregionid { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("regionid")]
    public int Regionid { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("PhysicianRegions")]
    public virtual Physician Physician { get; set; } = null!;

    [ForeignKey("Regionid")]
    [InverseProperty("PhysicianRegions")]
    public virtual Region Region { get; set; } = null!;
}
