using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DataModels;

[Table("encounterform")]
public partial class Encounterform
{
    [Key]
    public int EncounterFormId { get; set; }

    public int? RequestId { get; set; }

    public string? HistoryOfPresentIllnessOrInjury { get; set; }

    public string? MedicalHistory { get; set; }

    public string? Medications { get; set; }

    public string? Allergies { get; set; }

    public string? Temp { get; set; }

    [Column("HR")]
    public string? Hr { get; set; }

    [Column("RR")]
    public string? Rr { get; set; }

    public string? BloodPressureSystolic { get; set; }

    public string? BloodPressureDiastolic { get; set; }

    public string? O2 { get; set; }

    public string? Pain { get; set; }

    public string? Heent { get; set; }

    [Column("CV")]
    public string? Cv { get; set; }

    public string? Chest { get; set; }

    [Column("ABD")]
    public string? Abd { get; set; }

    public string? Extremeties { get; set; }

    public string? Skin { get; set; }

    public string? Neuro { get; set; }

    public string? Other { get; set; }

    public string? Diagnosis { get; set; }

    public string? TreatmentPlan { get; set; }

    public string? MedicationsDispensed { get; set; }

    public string? Procedures { get; set; }

    public string? FollowUp { get; set; }

    public int? AdminId { get; set; }

    public int? PhysicianId { get; set; }

    public bool IsFinalize { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("Encounterforms")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("Encounterforms")]
    public virtual Physician? Physician { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("Encounterforms")]
    public virtual Request? Request { get; set; }
}
