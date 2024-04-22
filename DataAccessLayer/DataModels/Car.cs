using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DataModels;

[Keyless]
[Table("cars")]
public partial class Car
{
    [Column("brands")]
    [StringLength(23)]
    public string? Brands { get; set; }

    [Column("model")]
    [StringLength(23)]
    public string? Model { get; set; }

    [Column("color")]
    [StringLength(34)]
    public string? Color { get; set; }
}
