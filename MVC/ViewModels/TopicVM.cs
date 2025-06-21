using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
    public class TopicVM
    {
        public int Id { get; set; }
        [Required]
        [Display(Name ="Topic Title")]
        [StringLength(100)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage ="A category must be selected")]
        [Display(Name ="Select Category")]
        public int CategoryId { get; set; }

        [Display(Name ="Category")]
        public string CategoryName { get; set; }


        [Display(Name ="Select Tags")]
        public List<int> TagIds{ get; set; }

        [Display(Name ="Tags")]
        public List<string> TagNames { get; set; }

        public int PostsCount { get; set; }

        [Display(Name ="Publish Date")]
        public DateTime? Publish_Date { get; set; }
    }
}
