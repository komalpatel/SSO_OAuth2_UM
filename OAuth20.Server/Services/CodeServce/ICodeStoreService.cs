using OAuth20.Server.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace OAuth20.Server.Services.CodeServce
{
    public interface ICodeStoreService
    {
        string GenerateAuthorizationCode(AuthorizationCode authorizationCode);
        AuthorizationCode GetClientDataByCode(string key);
        AuthorizationCode UpdatedClientDataByCode(string key, ClaimsPrincipal claimsPrincipal, IList<string> requestdScopes);
        AuthorizationCode RemoveClientDataByCode(string key);
    }
}
