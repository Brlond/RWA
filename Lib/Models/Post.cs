using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lib.Models;

[Table("Post")]
public partial class Post
{
    [Key]
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? TopicId { get; set; }

    [Column(TypeName = "text")]
    public string Content { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? PostedAt { get; set; }

    public bool? Approved { get; set; }

    [InverseProperty("Post")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    [ForeignKey("TopicId")]
    [InverseProperty("Posts")]
    public virtual Topic? Topic { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Posts")]
    public virtual User? User { get; set; }
}
