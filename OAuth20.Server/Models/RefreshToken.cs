using System;

namespace OAuth20.Server.Models
{
    public class RefreshToken
    {
        public string? Token {get;set;}
        public DateTime Created {get;set;}
        public DateTime Expires {get;set;}


    }
}