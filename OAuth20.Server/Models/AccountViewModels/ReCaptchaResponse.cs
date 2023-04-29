using Newtonsoft.Json;

namespace OAuth20.Server.Models.AccountViewModels
{
    public class ReCaptchaResponse

    {

        [JsonProperty("success")]

        public bool Success { get; set; }



        [JsonProperty("score")]

        public float Score { get; set; }

        [JsonProperty("error_codes")]

        public string[] ErrorCodes { get; set; }

    }

}