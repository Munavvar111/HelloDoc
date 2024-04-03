using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DataModels;

[Table("healthprofessionals")]
public partial class Healthprofessional
{
    [Key]
    [Column("vendorid")]
    public int Vendorid { get; set; }

    [Column("vendorname")]
    [StringLength(100)]
    public string Vendorname { get; set; } = null!;

    [Column("healthprofessionalid")]
    public int? Healthprofessionalid { get; set; }

    [Column("faxnumber")]
    [StringLength(50)]
    public string Faxnumber { get; set; } = null!;

    [Column("address")]
    [StringLength(150)]
    public string? Address { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string? City { get; set; }

    [Column("state")]
    [StringLength(50)]
    public string? State { get; set; }

    [Column("zip")]
    [StringLength(50)]
    public string? Zip { get; set; }

    [Column("regionid")]
    public int? Regionid { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("phonenumber")]
    [StringLength(100)]
    public string? Phonenumber { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("businesscontact")]
    [StringLength(100)]
    public string? Businesscontact { get; set; }

    [Column("isdeleted")]
    public bool? Isdeleted { get; set; }

    [ForeignKey("Healthprofessionalid")]
    [InverseProperty("Healthprofessionals")]
    public virtual Healthprofessionaltype? HealthprofessionalNavigation { get; set; }
}
