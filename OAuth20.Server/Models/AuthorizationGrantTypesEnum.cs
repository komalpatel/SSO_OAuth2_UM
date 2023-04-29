using System.ComponentModel;

namespace OAuth20.Server.Models
{
    internal enum AuthorizationGrantTypesEnum : byte
    {
        [Description("code")]
        Code,

        [Description("clientcredentials")]
        ClientCredentials,

        [Description("refreshtoken")]
        RefreshToken
    }
}
