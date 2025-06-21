using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lib.Models;

[Table("User")]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(255)]
    public string PasswordSalt { get; set; } = null!;

    [StringLength(256)]
    public string FirstName { get; set; } = null!;

    [StringLength(256)]
    public string LastName { get; set; } = null!;

    [StringLength(256)]
    public string? Email { get; set; }

    public bool? IsAdmin { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    [InverseProperty("User")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
