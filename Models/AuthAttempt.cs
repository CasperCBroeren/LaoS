namespace LaoS.Models
{
    public class AuthAttempt
    {
        public string Access_Token { get; set; }
        public string Scope { get; set; }
        public string Team_Name { get; set; }
        public string Team_Id { get; set; }
        public IncomingWebhook Incoming_Webhook { get; set; }
    }

    public class IncomingWebhook
    {
        public string Url { get; set; }
        public string Channel { get; set; }
        public string Configuration_Url { get; set; }
    }
}
