using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.DataModels;

[Table("physician_notification")]
public partial class PhysicianNotification
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("isnotificationstopped", TypeName = "bit(1)")]
    public BitArray Isnotificationstopped { get; set; } = null!;

    [ForeignKey("Physicianid")]
    [InverseProperty("PhysicianNotifications")]
    public virtual Physician Physician { get; set; } = null!;
}
