using System.Collections.Generic;

namespace LaoS.Models
{
    public class Message
    {
        public string Type { get; set; }

        public string Subtype { get; set; }
        public string Channel { get; set; }
        public string User { get; set; }

        public string Ts { get; set; }
         
        public string Text { get; set; }

        public Message Edited { get; set; }

        public string DeletedTs { get; set; }
       
    }
}
