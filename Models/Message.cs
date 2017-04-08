using System.Collections.Generic;

namespace LaoS.Models
{
    public class Message
    {
        public string Type { get; set; }

        public string Subtype { get; set; }

        public bool Hidden { get; set; }
                
        public string Channel { get; set; }

        public string User { get; set; }

        public string Ts { get; set; }

        public bool Is_Starred { get; set; }

        public List<string> Pinned_To { get; set; }

        public List<Reaction> Reactions { get; set; }

        public string Text { get; set; }
        
        public string Deleted_Ts { get; set; }

        public string Event_Ts { get; set; }

        public EditAction Edited { get; set; }
    }
}
