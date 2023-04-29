using System;
using System.ComponentModel.DataAnnotations;

namespace OAuth20.Server.Models
{

    public class User 
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        public string? Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }


        public string? GuID { get; set; }

        public string token { get; set; }

        //public string? RefreshToken { get; set; }
        //public DateTime RefreshTokenExpiryTime { get; set; }


    }
}
