using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lib.Models;

[Table("Rating")]
[Index("UserId", "PostId", Name = "UQ_User_Post", IsUnique = true)]
public partial class Rating
{
    [Key]
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? PostId { get; set; }

    public int? Score { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RatedAt { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("Ratings")]
    public virtual Post? Post { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Ratings")]
    public virtual User? User { get; set; }
}
