using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OAuth20.Server.Models.AccountViewModels
{
    public class CreateUserRequest
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "Das {0} muss mindestens {2} und maximal {1} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public DateTime lastLoginDate { get; set; }
        
        [Display(Name = "I accept the above terms and conditions.")]
        [Required(ErrorMessage = "Please accept the terms and condition.")]
        public bool IsChecked { get; set; }

        
    }
}
