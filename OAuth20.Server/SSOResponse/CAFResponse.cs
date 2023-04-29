using System;
using System.ComponentModel.DataAnnotations;

namespace OAuth20.Server.SSOResponse
{
    public class CAFResponse
    {

        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }


        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        public string LastName { get; set; }

        public string Token { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string GuID { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public string TokenType { get; set; }
        public string Status { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
