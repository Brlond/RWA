using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name can't be longer than 100 characters.")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }


        [Display(Name = "Number of Topics")]
        public int? TopicCount { get; set; }
    }
}
