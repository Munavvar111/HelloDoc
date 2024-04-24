using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("Category")]
public partial class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Column(TypeName = "character varying")]
    public string CategoryName { get; set; } = null!;

    [InverseProperty("CategoryNavigation")]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
