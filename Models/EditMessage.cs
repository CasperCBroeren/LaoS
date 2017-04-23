using System;
using System.Collections.Generic;
using System.Text;

namespace LaoS.Models
{
    public class EditMessage : SlackMessage
    {
        public EditAction Edited { get; set; }
    }
}
