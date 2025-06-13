using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lib.Models;

[Table("Tag")]
[Index("Name", Name = "UQ__Tag__737584F6347E0865", IsUnique = true)]
public partial class Tag
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("Tags")]
    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
