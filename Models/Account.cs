using Microsoft.WindowsAzure.Storage.Table;

namespace LaoS.Models
{
    public class Account : TableEntity
    {
        public Account()
        {

        }

        public Account(string team_id, string name, string slackToken)  
        {
            PartitionKey = "DEV";
            RowKey = team_id;
            Name = name;
            SlackToken = slackToken;             
        }

        public string TeamId { get { return RowKey; } set { RowKey = value; } }
        public string Name { get; set; }
        public string SlackToken { get; set; }
        public string ChannelId { get; set; }
        
    }
}
