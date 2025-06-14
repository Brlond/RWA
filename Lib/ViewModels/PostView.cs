using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.ViewModels
{
    public class PostView
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Username { get; set; }
        public string TopicTitle { get; set; }
        public DateTime PostedAt { get; set; }
        public bool Approved { get; set; }

        public List<int> Scores { get; set; } = new();
    }
}
