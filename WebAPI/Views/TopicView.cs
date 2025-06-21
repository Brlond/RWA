using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.ViewModels
{
    public class TopicView
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<string> TagNames { get; set; } = new();
        public List<PostPreview> Posts { get; set; } = new();
    }


    public class PostPreview
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? Username { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
