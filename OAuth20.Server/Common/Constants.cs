﻿namespace OAuth20.Server.Common
{
    public static class Constants
    {
        public static string Plain = "plain";
        public static string SHA256 = "S256";

        // OpenID Connect Scopes
        public static string OpenId = "openid";
        public static string Profile = "profile";
        public static string Email = "email";
        public static string Address = "address";
        public static string Phone = "phone";

        // Token Types
        public static class TokenTypes
        {
            public const string JWTAcceseccToken = "Access_Token";
            public const string JWTIdentityToken = "Idp_Token";
            public const string AccessAndIdpToken = "Access_Idp_Token";
        }




        public static class Statuses
        {
            public const string InActive = "inactive";
            public const string Revoked = "revoked";
            public const string Valid = "valid";
        }
    }
}
