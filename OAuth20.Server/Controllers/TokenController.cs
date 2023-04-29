using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuth20.Server.Models;
using OAuth20.Server.Models.Context;
using OAuth20.Server.Services.Token;
using OAuth20.Server.SSORequest;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OAuth20.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly BaseDBContext _userContext;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        public TokenController(BaseDBContext userContext, ITokenService tokenService, UserManager<AppUser> userManager)
        {
            this._userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            this._tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userManager = userManager;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");

            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value; //this is mapped to the Name claim by default

            //var user = _userManager.FindByNameAsync(username);
            //var UName = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;

            var user =  await _userContext.OAuthTokens.Where(e => e.UserName == username).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime >= DateTime.Now)
                return BadRequest("Invalid client request");

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshTokenExpiryTime= DateTime.UtcNow.AddMonths(5);
            user.RefreshToken = newRefreshToken;
            _userContext.SaveChanges();

            return Ok(new AuthenticatedResponse()
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;

            var user = _userContext.OAuthTokens.SingleOrDefault(u => u.UserName == username);
            if (user == null) return BadRequest();

            user.RefreshToken = null;

            _userContext.SaveChanges();

            return NoContent();
        }
    }
}
