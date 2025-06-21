namespace MVC.ViewModels
{
    public class SearchVM
    {
        public string Query { get; set; }
        public string OrderBy { get; set; }
        public int Page { get; set; } = 1;
        public int LastPage { get; set; }
        public int FromPager { get; set; }
        public int ToPager { get; set; }
        public int Size { get; set; } = 10;
        public List<TopicVM> Topics { get; set; }

    }
}
