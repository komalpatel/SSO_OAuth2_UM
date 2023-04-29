using System.ComponentModel.DataAnnotations;

namespace OAuth20.Server.Models.ManageViewModels
{
    public class ChangeProfileViewModel
    {

        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        public string LastName { get; set; }
               
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
