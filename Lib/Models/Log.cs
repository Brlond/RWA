using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lib.Models;

public partial class Log
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateOf { get; set; }

    public int? Severity { get; set; }

    [StringLength(1024)]
    public string? Message { get; set; }

    public string? ErrorText { get; set; }
}
