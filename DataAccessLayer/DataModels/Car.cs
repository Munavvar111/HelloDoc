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
    [Column("parts")]
    public int? Parts { get; set; }

    [Column("carname", TypeName = "character varying")]
    public string? Carname { get; set; }
}
