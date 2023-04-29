using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(config =>
{
    config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
 .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
 {
     // this is my Authorization Server Port
     options.Authority = "https://localhost:7275";
     options.ClientId = "ChargeatFriends";
     options.ClientSecret = "123456789";
     options.ResponseType = "code";
     options.CallbackPath = "/signin-oidc";
     options.SaveTokens = true;
     //options.TokenValidationParameters = new TokenValidationParameters
     //{
     //    ValidateIssuerSigningKey = false,
     //    SignatureValidator = delegate (string token, TokenValidationParameters validationParameters)
     //    {
     //        var jwt = new JwtSecurityToken(token);
     //        return jwt;
     //    },
     //};
 });
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapDefaultControllerRoute();
//});


app.Run();