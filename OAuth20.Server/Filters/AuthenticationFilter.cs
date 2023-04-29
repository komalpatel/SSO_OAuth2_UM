using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OAuth20.Server.Filters
{

    public class AuthenticationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var token = context.HttpContext.Request.Headers["Token"][0];

                var key = Encoding.ASCII.GetBytes("4gSd0AsIoPvyD3PsXYNrP2XnVpIYCLLL");
                var handler = new JwtSecurityTokenHandler();
                var tokenSecure = handler.ReadToken(token) as SecurityToken;
                var validations = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var claims = handler.ValidateToken(token, validations, out tokenSecure);
            }
            catch (Exception Ex)
            {
                context.Result = new ObjectResult("LoginAgain") { StatusCode = 203 };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }
    }
}
