using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DataModels;

[Table("blockrequests")]
public partial class Blockrequest
{
    [Key]
    [Column("blockrequestid")]
    public int Blockrequestid { get; set; }

    [Column("phonenumber")]
    [StringLength(50)]
    public string? Phonenumber { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("reason", TypeName = "character varying")]
    public string? Reason { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("isactive")]
    public bool? Isactive { get; set; }

    [ForeignKey("Requestid")]
    [InverseProperty("Blockrequests")]
    public virtual Request Request { get; set; } = null!;
}
