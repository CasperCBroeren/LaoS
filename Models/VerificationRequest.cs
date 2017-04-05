
namespace LaoS.Models
{
    public class VerificationRequest
    {
        public string Token { get; set; }
        public string Challenge { get; set; }

        public string Type { get; set; }
    }
}
