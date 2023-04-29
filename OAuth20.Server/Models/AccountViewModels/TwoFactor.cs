using System.ComponentModel.DataAnnotations;

namespace OAuth20.Server.Models.AccountViewModels
{
    public class TwoFactor
    {
        [Required]
        public string TwoFactorCode { get; set; }
    }
}
