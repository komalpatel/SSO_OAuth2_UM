using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Azure.Documents;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.UriParser;
using OAuth20.Server.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace OAuth20.Server.Models
{
   
    public class ClientStore
    {
     
        private readonly OAuthApplicationEntity _authDbContext;
        //public ClientStore(OAuthApplicationEntity authDbContext)
        //{
        //    _authDbContext = authDbContext;


        //    var atoken = new OAuthApplicationEntity
        //    {
        //        ClientId = _authDbContext.ClientId,
        //        ClientName = _authDbContext.ClientName,
        //        ClientSecret = _authDbContext.ClientSecret,
        //        ClientUri = _authDbContext.ClientUri,
        //        ApplicationId = _authDbContext.ApplicationId,
        //        IsActive = true,
        //        RedirectUris = _authDbContext.RedirectUris,
        //    };

        //}
        public void Client(OAuthApplicationEntity context)
        {
            var clientname = context.ClientName;
            var clientsecret = context.ClientSecret;    
            var clientid = context.ClientId;
            var clientredirecturl = context.RedirectUris;
            var appid = context.ApplicationId;




        }
        public IEnumerable<Client> Clients = new[]
        {
                     new Client
                     {
                         ClientName = "CHF",
                         ClientId = "CHF",
                         ClientSecret = "123456789",
                         AllowedScopes = new[]{ "openid", "profile"},
                         GrantType = GrantTypes.Code,
                         IsActive = true,
                         ClientUri = "https://localhost:5001",
                         RedirectUri = "https://localhost:5001/signin-oidc"
                     },

                     new Client
                         {
                             ClientName = "ChargeFriends",
                             ClientId = "ChargeatFriends",
                             ClientSecret = "123456789",
                             AllowedScopes = new[]{ "openid", "profile"},
                             GrantType = GrantTypes.Code,
                             ClientUri = "https://localhost:44317",
                             RedirectUri = "https://localhost:44317/signin-oidc",
                             IsActive = true,
                             UsePkce = true,
                     },

                     new Client
                         {
                             ClientName = "ChargeFriendsReact",
                             ClientId = "ChargeatFriendsReact",
                             ClientSecret = "123456789",
                             AllowedScopes = new[]{ "openid", "profile"},
                             GrantType = GrantTypes.Code,
                             ClientUri = "https://localhost:3000",
                             RedirectUri = "https://localhost:3000/signin-oidc",
                             IsActive = true,
                             UsePkce = true,
                     }


            //new Client
            //{
            //    ClientName = "ChargeFriends",
            //    ClientId = "ChargeatFriends",
            //    ClientSecret = "123456789",
            //    AllowedScopes = new[]{ "openid", "profile"},
            //    GrantType = GrantTypes.Code,
            //    IsActive = true,
            //    ClientUri = "https://localhost:7160",
            //    RedirectUri = "https://localhost:7160/signin-oidc"
            //}

        };
    }
}
