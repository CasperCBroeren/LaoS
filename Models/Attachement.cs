using System.Collections.Generic;

namespace LaoS.Models
{
    public class Attachment
    {

        public string Fallback { get; set; }

        public string Color { get; set; }

        public string Pretext { get; set; }

        public string Author_Name { get; set; }

        public string Author_Link { get; set; }

        public string Author_Icon { get; set; }

        public string Title { get; set; }

        public string Title_Link { get; set; }

        public string Text { get; set; }

        public List<AttachmentField> Fields { get; set; }

        public string Image_Url { get; set; }
         
        public string Thumb_Url { get; set; }

        public string Footer { get; set; }

        public string Footer_Icon { get; set; }

        public string Service_Icon { get; set; }

        public string Service_Name { get;   set; }

        public double Ts { get; set; }
        

    }
}
