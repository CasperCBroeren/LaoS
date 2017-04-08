using System.Collections.Generic;

namespace LaoS.Models
{
    public class UserResponse
    {
        public bool Ok { get; set; }
        public List<User> Members { get; set; }
    }
}
