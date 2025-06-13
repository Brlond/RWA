using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lib.Models;

[Table("UserAccount")]
[Index("Username", Name = "UQ__UserAcco__536C85E4D31ED504", IsUnique = true)]
public partial class UserAccount
{
    [Key]
    public int UserId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string PasswordHash { get; set; } = null!;

    public int? PasswordSalt { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    public bool? IsAdmin { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    [InverseProperty("User")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
