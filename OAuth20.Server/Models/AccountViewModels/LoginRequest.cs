namespace OAuth20.Server.Models.AccountViewModels
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

        public bool Remember { get; set; }

    }
}
