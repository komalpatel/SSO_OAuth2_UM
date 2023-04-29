using System.Collections.Generic;

namespace OAuth20.Server.SSOResponse
{
    public class CreateProfileResponse
    {
        public bool Succeeded { get; set; }
        public string Error { get; set; } = string.Empty;
        public string ErrorDescription { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);

        public IEnumerable<object> Errors { get; internal set; }
    }
}
