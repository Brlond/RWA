using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lib.Models;

[Table("Topic")]
public partial class Topic
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Topics")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Topic")]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    [ForeignKey("TopicId")]
    [InverseProperty("Topics")]
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
