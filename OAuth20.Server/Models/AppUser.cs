using Microsoft.AspNetCore.Identity;
using System;

namespace OAuth20.Server.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string LastLoginDate { get; set; }


 
    }
}
