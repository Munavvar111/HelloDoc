using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DataModels;

[Table("aspnet_roles")]
public partial class AspnetRole
{
    [Column("name")]
    [StringLength(256)]
    public string Name { get; set; } = null!;

    [Key]
    [Column("id")]
    public int Id { get; set; }
}
