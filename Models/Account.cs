using Microsoft.WindowsAzure.Storage.Table;

namespace LaoS.Models
{
    public class Account : TableEntity
    {
        public Account()
        {

        }

        public Account(string laosID, string name, string slackToken)  
        {
            PartitionKey = "DEV";
            RowKey = laosID;
            Name = name;
            SlackToken = slackToken;
        }

        public string LaosID { get { return RowKey; } set { RowKey = value; } }
        public string Name { get; set; }
        public string SlackToken { get; set; }
    }
}
