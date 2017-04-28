using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClarifaiApiClient.Models
{
    public class Token
    {
        public string Access_Token { get; set; }
        public int Expires_In { get; set; }
        public string Scope { get; set; }
        public string Token_Type { get; set; }
    }
    public class TokenError
    {
        public string Status_Code { get; set; }
        public string Status_Msg { get; set; }
    }
}
