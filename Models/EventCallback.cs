using System.Collections.Generic;

namespace LaoS.Models
{
    public class EventCallback<T> 
    {
        public string Token { get; set; }        
        public string Team_Id { get; set; }
        public string Api_App_Id { get; set; }        
        public T Event { get; set; }
        public string Type { get; set; }
        public List<string> Authed_Users { get; set; }        
        public string Event_Id { get; set; }
        public int Event_Time { get; set; }        
    }
}
