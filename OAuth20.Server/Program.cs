
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OAuth20.Server.Configuration;
using OAuth20.Server.Helpers;
using OAuth20.Server.Models;
using OAuth20.Server.Models.AccountViewModels;
using OAuth20.Server.Models.Context;
using OAuth20.Server.Services;
using OAuth20.Server.Services.CodeServce;
using OAuth20.Server.Services.Token;
using OAuth20.Server.Services.Users;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
var configServices = builder.Configuration;
var connectionString = builder.Configuration.GetConnectionString("BaseDBConnection");
var Corsopenpolicy = "OpenCORSPolicy";

builder.Services.AddDbContext<BaseDBContext>(op =>
{
    op.UseSqlServer(connectionString);
});


builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 7;
    opt.Password.RequireDigit = false;
    opt.Password.RequireUppercase = false;
    opt.User.RequireUniqueEmail = true;
    opt.Lockout.MaxFailedAccessAttempts = 5;
    opt.User.RequireUniqueEmail = true;
})
 .AddEntityFrameworkStores<BaseDBContext>()
 .AddDefaultTokenProviders();
//builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<BaseDBContext>().AddDefaultTokenProviders();
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
   opt.TokenLifespan = TimeSpan.FromDays(10));

//builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
//{
//    options.SignIn.RequireConfirmedEmail = false;
//    options.Password.RequireDigit = true;
//    options.Password.RequireLowercase = true;
//    options.Password.RequiredLength = 8;
//    options.Password.RequireUppercase = false;
//    options.Password.RequireNonAlphanumeric = false;
//    options.Lockout.MaxFailedAccessAttempts = 5;
//    options.User.RequireUniqueEmail = true;
//}).AddRoles<IdentityRole>().AddEntityFrameworkStores<BaseDBContext>().AddDefaultTokenProviders();

// Register the ConfigurationBuilder instance of AuthSettings
var authSettings = builder.Configuration.GetSection(nameof(AuthSettings));
builder.Services.Configure<AuthSettings>(authSettings);

//var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings[nameof(AuthSettings.SecretKey)]));

// jwt wire up
// Get options from app settings
//var jwtAppSettingOptions = builder.Configuration.GetSection(nameof(JwtIssuerOptions));

// Configure JwtIssuerOptions
//builder.Services.Configure<JwtIssuerOptions>(options =>
//{
//    options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
//    options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
//    options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
//});

//var tokenValidationParameters = new TokenValidationParameters
//{
//    ValidateIssuer = true,
//    ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

//    ValidateAudience = true,
//    ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

//    ValidateIssuerSigningKey = true,
//    IssuerSigningKey = signingKey,

//    RequireExpirationTime = false,
//    ValidateLifetime = true,
//    ClockSkew = TimeSpan.Zero
//};
builder.Services.AddOptions<CaptchaSettings>().BindConfiguration("Captcha");
builder.Services.AddTransient<CaptchaVerificationService>();
builder.Services.AddTransient<ITokenService, OAuth20.Server.Services.Token.TokenService>();

// jwt wire up
// Get options from app settings
//var jwtAppSettingOptions = builder.Configuration.GetSection(nameof(JwtIssuerOptions));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        RequireExpirationTime = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false;
//    options.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuer = true,
//        ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

//        ValidateAudience = true,
//        ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = signingKey,

//        RequireExpirationTime = false,
//        ValidateLifetime = true,
//        ClockSkew = TimeSpan.Zero

//    };
//    options.Events = new JwtBearerEvents
//    {
//        OnAuthenticationFailed = context =>
//        {
//            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
//            {
//                context.Response.Headers.Add("Token-Expired", "true");
//            }
//            return Task.CompletedTask;
//        }
//    };
// });
//builder.Services.AddAuthentication();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Accounts/Login";
    options.LogoutPath = "/Accounts/Logout";
    options.LogoutPath = "/Accounts/Register";
    options.AccessDeniedPath = "/Accounts/AccessDenied";
    options.Cookie.SameSite= Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromDays(5);
    options.SlidingExpiration = true;
});



//builder.Services.AddAuthentication()
//        .AddGoogle(opts =>
//        {
//            opts.ClientId = "141297829880-0et15f1ajjlqobf94ajqd5hcd6airca3.apps.googleusercontent.com";
//            opts.ClientSecret = "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";
//            opts.SignInScheme = IdentityConstants.ExternalScheme;
//        });

//builder.Services.Configure<OAuthOptions>(configServices.GetSection("EmailConfiguration"));
//builder.Services.AddSingleton(emailConfig);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: Corsopenpolicy, corsBuilder =>
    {
        corsBuilder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("*");
    });
});


builder.Services.Configure<OAuthOptions>(configServices.GetSection("OAuthOptions"));
builder.Services.AddScoped<IAuthorizeResultService, AuthorizeResultService>();
builder.Services.AddSingleton<ICodeStoreService, CodeStoreService>();
builder.Services.AddScoped<IUserManagerService, UserManagerService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseQueryStrings = true;
    options.LowercaseUrls = true;
});

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
    
});


builder.Services.AddControllersWithViews().AddViewLocalization(); 

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    
    var cultures = new CultureInfo[]
    {
                    new CultureInfo("de-DE"),
                    new CultureInfo("en-GB")
                    //new CultureInfo("fr-FR")
    };
    options.DefaultRequestCulture = new RequestCulture(culture: "de-DE", uiCulture: "de-DE");

    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
    options.SetDefaultCulture("de-DE");

});

builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.IgnoreObsoleteActions();
    c.IgnoreObsoleteProperties();
    c.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

//app.UseSwaggerUI(c => {
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AzureAD_OAuth_API v1");
//    //c.RoutePrefix = string.Empty;    
//    c.OAuthClientId("Client Id");
//    c.OAuthClientSecret("Client Secret Key");
//    c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
//});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");

});
//app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BasicAuth v1"));
app.UseRequestLocalization();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Accounts}/{action=Index}/{id?}");
//app.MapControllerRoute(
//    name: "profile",
//    pattern: "{controller=Accounts}/{action=Profile}/{id?}");



//app.UseEndpoints(endpoints =>
//{
//	endpoints.MapControllerRoute(
//		name: "default",
//		pattern: "{controller=Home}/{action=Index}/{id?}");
//	endpoints.MapRazorPages();
//});

//app.MapRazorPages();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Accounts}/{action=Index}/{id?}");
//    endpoints.MapControllerRoute(
//        name: "profile",
//        pattern: "{controller=Accounts}/{action=Profile}/{id?}");
//    endpoints.MapRazorPages();
//});
app.UseCors(Corsopenpolicy);

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
