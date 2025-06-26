using Lib.Models;
using MVC.ViewModels;

namespace MVC.ViewModels
{
    public class PostSearchVM
    {
        public string OrderBy { get; set; }
        public int Page { get; set; } = 1;
        public int LastPage { get; set; }
        public int FromPager { get; set; }
        public int ToPager { get; set; }
        public int Size { get; set; } = 5;
        public List<PostVM> Posts { get; set; }
        public TopicVM Topic { get; set; }
    }
}
