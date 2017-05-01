using Microsoft.WindowsAzure.Storage.Table;

namespace LaoS.Models
{
    public class Account : TableEntity
    {
        public Account()
        {

        }

        public Account(string laosID, string name, string slackToken, string channel)  
        {
            PartitionKey = "DEV";
            RowKey = laosID;
            Name = name;
            SlackToken = slackToken;
            Channel = channel;
        }

        public string LaosID { get { return RowKey; } set { RowKey = value; } }
        public string Name { get; set; }
        public string SlackToken { get; set; }
        public string Channel { get; set; }
    }
}
