using OAuth20.Server.Common;
using System.Collections.Generic;

namespace OAuth20.Server.Models
{
    public class GrantTypes
    {
        public static IList<string> Code =>
            new[] { AuthorizationGrantTypesEnum.Code.GetEnumDescription() };

        public static IList<string> ClientCredentials =>
            new[] { AuthorizationGrantTypesEnum.ClientCredentials.GetEnumDescription() };

        public static IList<string> RefreshToken =>
            new[] { AuthorizationGrantTypesEnum.RefreshToken.GetEnumDescription() };
    }
}
