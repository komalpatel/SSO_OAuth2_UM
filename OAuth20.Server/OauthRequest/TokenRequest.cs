﻿using System.Collections.Generic;

namespace OAuth20.Server.OauthRequest
{
    public class TokenRequest
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string code { get; set; }
        public string grant_type { get; set; }
        public string redirect_uri { get; set; }
        public string code_verifier { get; set; }
        public IList<string> scopes { get; set; }
    }
}
