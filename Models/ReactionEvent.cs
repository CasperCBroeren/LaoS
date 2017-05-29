namespace LaoS.Models
{
    public class ReactionEvent
    {
        public string Type { get; set; }
        public string User { get; set; }
        public string Reaction { get; set; }
        public string Item_User { get; set; }
        public string Event_Ts { get; set; }
        public ReactionItem Item { get; set; }
    }

    public class ReactionItem
    {
        public string Type { get; set; }
        public string Channel { get; set; }
        public string Ts { get; set; }
        public string File { get; set; }
        public string File_Comment { get; set; }
    }
}
