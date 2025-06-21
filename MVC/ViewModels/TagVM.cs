using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
    public class TagVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Tag Name is required.")]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "Number of Topics")]
        public int? TopicCount { get; set; }

    }
}
