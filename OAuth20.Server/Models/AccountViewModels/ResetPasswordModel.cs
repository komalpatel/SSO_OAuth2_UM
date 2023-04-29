using System.ComponentModel.DataAnnotations;

namespace OAuth20.Server.Models.AccountViewModels
{
    public class ResetPasswordModel
    {

        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Code { get; set; }
        // public string Code { get; set; }


    }
}
