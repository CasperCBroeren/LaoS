using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Globalization;

namespace LaoS.Models
{
    public class SlackMessage : TableEntity
    {
        public static readonly CultureInfo DecimalFormat = new CultureInfo("en-US");
        public SlackMessage()
        {

        }
        
        public string Type { get; set; }

        public string Subtype { get; set; }

        public bool Hidden { get; set; }

        private string channel;
        public string Channel { get
            {
                return channel;
            }
            set
            {
                channel = value;
                this.PartitionKey = value;
            }
        }

        public string User { get; set; }

        public User FullUser { get; set; }  

        public string Ts { get; set; }

        public bool Is_Starred { get; set; }

        public List<string> Pinned_To { get; set; }

        public List<Reaction> Reactions { get; set; }

        public string Text { get; set; }
        
        public string Deleted_Ts { get; set; }

        private string event_Ts;
        public string Event_Ts { get
            {
                return event_Ts;
            }
            set
            {
                event_Ts = value;
                RowKey = value;
            }
        }
        
        public EditMessage Message { get; set; }

        public List<Attachment> Attachments { get; set; }

        public List<Link> Links { get; set; }

        public SlackMessage Previous_Message { get;  set; }
    }
}
