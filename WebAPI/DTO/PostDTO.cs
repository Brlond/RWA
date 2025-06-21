using Lib.Models;

namespace WebApp.DTO
{
    public class PostDTO
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public int TopicId { get; set; }
        public string Content { get; set; }
        public List<int>? Scores { get; set; }
    }
}
