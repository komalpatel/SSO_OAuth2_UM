using System.ComponentModel.DataAnnotations;

namespace OAuth20.Server.Models.AccountViewModels
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
