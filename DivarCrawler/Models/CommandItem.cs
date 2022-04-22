using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivarCrawler.Models
{
    public class CommandItem
    {
        public int Index { get; set; }
        public string Command { get; set; }
        public Func<Task> Method { get; set; }
    }
}
