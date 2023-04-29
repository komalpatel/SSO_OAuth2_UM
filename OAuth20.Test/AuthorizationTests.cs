using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OAuth20.Server;
using OAuth20.Server.Controllers;
using OAuth20.Server.Models;
using OAuth20.Server.Models.Context;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.Services.Users;
using Xunit;

namespace OAuth20.Tests
{
    public class AuthorizationTests
    {
        [Fact]
        public void GivenAnAuthenticateModelInstance_WhenUserIsInTheDatabase_ThenGenerateABearerToken()
        {
            var credentials = new LoginRequest()
            {
                UserName = "bhargav@chargeatfriends.com",
                Password = "Welcome1?"
            };

            //var controller = CreateController();
            //var result = controller.Login(credentials);

            //Assert.NotNull(result);
            //Assert.IsType<OkObjectResult>(result);
            //Assert.Equal(200, ((ObjectResult)result).StatusCode);
            //Assert.NotNull(((ObjectResult)result).Value);
        }

        //[Fact]
        //public void GivenAnAuthenticateModelInstance_WhenUserIsNotInTheDatabase_ThenReturnUnauthorized()
        //{
        //    var credentials = new LoginRequest()
        //    {
        //        UserName = "bhargav@chargeatfriends.com",
        //        Password = "Welcome1?"
        //    };

        //    var controller = CreateController();
        //    var result = controller.Login(credentials);

        //    Assert.NotNull(result);
        //    Assert.IsType<UnauthorizedResult>(result);
        //    Assert.Equal(401, ((UnauthorizedResult)result).StatusCode);
        //}

        //[Fact]
        //public void GivenANullAuthenticateModelInstance_WhenUserIsNull_ThenReturnBadRequest()
        //{
        //    var controller = CreateController();
        //    var result = controller.Login(null);

        //    Assert.NotNull(result);
        //    Assert.IsType<BadRequestObjectResult>(result);
        //    Assert.Equal(400, ((ObjectResult)result).StatusCode);
        //}

        //public AccountsController CreateController()
        //{
        //    return new AccountsController(new BaseDBContext());
            
        //}
    }
}