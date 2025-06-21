using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
    public class PostVM
    {
        public int Id { get; set; }

        public string Content { get; set; }


        [Display(Name ="Author")]
        public string? CreatorUsername { get; set; }


        [Display(Name ="Published at")]
        public DateTime? Published_Date { get; set; }


        public int Score { get; set; }


        [Display(Name ="Select Topic")]
        public int TopicId { get; set; }


        [Display(Name ="Topic")]
        public string? TopicTitle { get; set; }
    }
}
