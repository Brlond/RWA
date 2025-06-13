using System.ComponentModel.DataAnnotations;

namespace WebApp.DTO
{
    public class LogDTO
    {
        public int? Id { get; set; }
        public int? Severity { get; set; }
        [StringLength(1024)]
        public string? Message { get; set; }
        public string? ErrorText { get; set; }
    }
}
