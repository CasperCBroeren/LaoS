using System.Collections.Generic;

namespace LaoS.Models
{
    public class Reaction
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public List<string> Users { get; set; }
    }
}
