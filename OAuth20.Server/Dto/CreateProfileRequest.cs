using System.ComponentModel.DataAnnotations;

namespace OAuth20.Server.Dto
{
    public class CreateProfileRequest
    {
        [Display(Name = "UserName")]
        public string UserName { get; set; }


        [Display(Name = "Email")]
        public string Email { get; set; }


        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
