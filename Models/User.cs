namespace LaoS.Models
{
   public class User
    {
        public string Id { get; set; }

        public string Team_Id { get; set; }

        public string Name { get; set; }

        public bool Deleted { get; set; }

        public string Status { get; set; }

        public string Color { get; set; }

        public string Real_Name { get; set; }

        public Profile Profile { get; set; }
    }
}
