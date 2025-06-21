using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.ViewModels
{
    public class TagView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> TopicTitles { get; set; } = new();
    }
}
