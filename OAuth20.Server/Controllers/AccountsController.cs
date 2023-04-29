using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OAuth20.Server.Helpers;
using OAuth20.Server.Models;
using OAuth20.Server.Models.AccountViewModels;
using OAuth20.Server.Models.Context;
using OAuth20.Server.Models.Entities;
using OAuth20.Server.OauthResponse;
using OAuth20.Server.Services.Users;
using OAuth20.Server.SSORequest;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using OAuth20.Server.Services.Token;
using Constants = OAuth20.Server.Common.Constants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Documents;

namespace OAuth20.Server.Controllers
{

    [EnableCors("OpenCORSPolicy")]

    public class AccountsController : Controller
    {
        private readonly IUserManagerService _userManagerService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly BaseDBContext _context;
        public const string APPLICATION_JSON = "application/json";
        private readonly CaptchaVerificationService _verificationService;
        private readonly ITokenService _tokenService;

        public AccountsController(IUserManagerService userManagerService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, BaseDBContext context, CaptchaVerificationService verificationService, ITokenService tokenService)
        {
            _userManagerService = userManagerService;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _verificationService = verificationService;
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }


        [Route("[controller]/[action]/")]
        [Route("[controller]/")]
        [Route("/")]
        [Route("/{id?}")]
        [Authorize]
        [HttpGet]
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {


                string url = HttpContext.Request.Path;
                string[] parts = url.Split('/');
                Guid guid = new Guid();
                if (Guid.TryParseExact(parts.Last(), "D", out guid))
                {
                    if (HttpContext.Request.Host.ToString().ToLower().Contains("app.chargeatfriends.com"))
                    {
                        return RedirectToAction("GotoStore", "ConnectMobile");
                    }
                }
                if (HttpContext.Request.Host.ToString().ToLower().Contains("app.chargeatfriends.com"))
                {
                    return Redirect("https://www.chargeatfriends.com");
                }

                ViewBag.IsFirst = false;
                if (!string.IsNullOrEmpty(User.Identity.Name))
                {
                    ViewBag.IsFirst = true;
                    //CustomerEntity customerEntity = await TableStorageHelper.Instance.GetCustomerEntityByEmail(User.Identity.Name);
                    //if (customerEntity != null && string.IsNullOrEmpty(customerEntity.StripeId))
                    //{
                    //	ViewBag.IsFirst = true;

                    //}
                }
            }
            return View();
        }

        private readonly CaptchaVerificationService verificationService;
        public string CaptchaClientKey { get; set; }

        [BindProperty(Name = "g-recaptcha-response")]
        public string CaptchaResponse { get; set; }

        [Route("[controller]/[action]/")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [Route("[controller]/[action]/")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            #region old login logic
            //var result = await _userManagerService.LoginUserAsync(request);
            //if (result.Succeeded)
            //    return RedirectToAction("Index", "Accounts");
            #endregion

            //var requestIsValid = true;
            //var requestIsValid = await this._verificationService.IsCaptchaValid(CaptchaResponse);

            //if (requestIsValid)
            //{
                try
                {

             
                AppUser appUser = await _userManager.FindByEmailAsync(request.UserName);
                if (appUser != null)
                {
                    await _signInManager.SignOutAsync();
                    var result1 = await _signInManager.PasswordSignInAsync(appUser, request.Password, request.Remember, false);

                    if (result1.Succeeded)
                        return Redirect(request.ReturnUrl ?? "/");
                       //return Content(System.Text.Json.JsonSerializer.Serialize(new { status = 200, isSuccess = true, message = "User Login successfully", UserDetails = appUser }), APPLICATION_JSON);


                    // uncomment Two Factor Authentication 
                    if (result1.RequiresTwoFactor)
                    {
                        //return RedirectToAction(nameof(LoginTwoStep), new { appUser.Email, request.ReturnUrl });
                        return RedirectToAction(nameof(LoginWithRecoveryCode), new { appUser.Email, request.ReturnUrl });
                        //return RedirectToAction("LoginWithRecoveryCode", new { appUser.Email, request.ReturnUrl });

                    }

                    // Uncomment Email confirmation
                    bool emailStatus = await _userManager.IsEmailConfirmedAsync(appUser);
                    if (emailStatus == false)
                    {
                        ModelState.AddModelError(nameof(request.UserName), "Email is unconfirmed, please confirm it first");
                    }

                    // locked user
                    /*if (result.IsLockedOut)
                        ModelState.AddModelError("", "Your account is locked out. Kindly wait for 10 minutes and try again");*/
                }
                ModelState.AddModelError(nameof(request.UserName), "Login Failed: Invalid Email or password");

                return View(request);
                }
                catch (Exception ex)
                {
                  return Unauthorized(ex.Message);
                }
            //}

            return Unauthorized("Failed to process captcha validation");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Accounts");


        }


        [Route("[controller]/[action]/")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [Route("[controller]/[action]/")]
        [HttpPost]
        public async Task<IActionResult> Register(CreateUserRequest request)
        {
            if(request.IsChecked==true)
            { 
            var result = await _userManagerService.CreateUserAsync(request);

            if (result.Succeeded)
                return RedirectToAction("Index", "Accounts");
            return View(request);
            }
            return View(request);
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Route("[controller]/[action]/")]
        [HttpPost]

        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed  
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // For more information on how to enable account confirmation and password reset please  
            // visit https://go.microsoft.com/fwlink/?LinkID=532713  
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = Base64UrlEncoder.Encode(code);

            var callbackUrl = $"{Request.Scheme}://{this.Request.Host}/Accounts/ResetPassword?code={code}&Email={user.Email}";

            //Console.WriteLine(HtmlEncoder.Default.Encode(callbackUrl));  
            //sendService.SendMail(input.Email, "Reset Password | Innovus", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendEmailPasswordReset(user.Email, $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
            //if (!ModelState.IsValid)
            //    return View(forgotPasswordModel);

            //var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            //if (user == null)
            //    return RedirectToAction(nameof(ForgotPasswordConfirmation));
            //var Code1 = await _userManager.GeneratePasswordResetTokenAsync(user);
            //var Code = Code1.ToLower();
            //var link = Url.Action(nameof(ResetPassword), "Accounts", new { Code, email = user.Email }, Request.Scheme);

            //EmailHelper emailHelper = new EmailHelper();
            //bool emailResponse = emailHelper.SendEmailPasswordReset(user.Email, link);
            //return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ResetPassword(string Code, string email)
        {
            var model = new ResetPasswordModel { Code = Code, Email = email };
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var code = resetPasswordModel.Code.Replace("%2F", "/").Replace("%2B", "+");
            if (!ModelState.IsValid)
                return View(resetPasswordModel);

            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
                RedirectToAction(nameof(ResetPasswordConfirmation));

            var resetPassResult = await _userManager.ResetPasswordAsync(user, Base64UrlEncoder.Decode(resetPasswordModel.Code), resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View();
            }

            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        #region google login
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Accounts");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction(nameof(Login));

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            string[] userInfo = { info.Principal.FindFirst(ClaimTypes.Name).Value, info.Principal.FindFirst(ClaimTypes.Email).Value };
            if (result.Succeeded)
                return View(userInfo);
            else
            {
                AppUser user = new AppUser
                {
                    Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
                };

                IdentityResult identResult = await _userManager.CreateAsync(user);
                if (identResult.Succeeded)
                {
                    identResult = await _userManager.AddLoginAsync(user, info);
                    if (identResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return View(userInfo);
                    }
                }
                return AccessDenied();
            }
        }

        #endregion

        #region start caf logic -below code return token as well validate token

        [Authorize]
        [Route("[controller]/[action]/")]
        [HttpPost]
        public async Task<ActionResult<Models.User>> GetUser(string username)
        {
            AppUser user = await _userManager.FindByNameAsync(username);


            var code = await _userManager.CreateSecurityTokenAsync(user);//.ResetAuthenticatorKeyAsync(user);

            var result = await _userManager.CreateAsync(user);
            if (user == null)
            {
                return BadRequest("Bad credentials");
            }

            return Json(new Models.User
            {
                Name = user.UserName,
                Email = user.Email,
                Password = user.PasswordHash,
                FirstName = user.FirstName,
                LastName = user.LastName,
                GuID = user.Id,


            });
        }

        /// <summary>
        /// Below is the CAF logic - below code return token and related information to the CAF backend applications
        /// </summary>
        /// <param name="CAFrequest"></param>
        /// <returns></returns>
        [Route("[controller]/[action]/")]
        [HttpPost]
        public async Task<IActionResult> CAFLogin([FromBody] CAFRequest CAFrequest)
        //public async Task<IActionResult> CAFLogin(OpenIdConnectLoginRequest loginRequest)
        {
            try
            {
                var result = new TokenResult();

                AppUser user = await _userManager.FindByNameAsync(CAFrequest.UserName);

                //User is not in SSO database then need to create 

                if (user == null)
                {

                    var CAFuser = new AppUser
                    {
                        FirstName = CAFrequest.FirstName,
                        LastName = CAFrequest.LastName,
                        UserName = CAFrequest.UserName,
                        Email = CAFrequest.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        PhoneNumber = CAFrequest.PhoneNumber,
                        TwoFactorEnabled = false,
                        Id = CAFrequest.GuID,
                        //PasswordHash = CAFrequest.Password,
                        LastLoginDate = DateTime.Now.ToString(),
                    };

                    var CreateCAFUser = await _userManager.CreateAsync(CAFuser, CAFrequest.Password);
                    if (CreateCAFUser.Succeeded)
                    {
                        user = await _userManager.FindByNameAsync(CAFuser.UserName);
                        //var CreateCAFUser =  _userManagerService.CreateUserAsync(CreateUserrequest);
                        EmailHelper emailHelper = new EmailHelper();
                        bool emailResponse = emailHelper.SendEmailRegister(CAFrequest.UserName);

                    }
                }

                // update or change password with new entry as well 

                if (user != null)
                {
                    var changepasword = await _userManager.ChangePasswordAsync(user, user.PasswordHash, CAFrequest.Password);

                }

                var tokenHandler = new JwtSecurityTokenHandler();

                #region old logic change on 13.04.2023

                //create a identity and add claims to the user which we want to log in
                //ClaimsIdentity claimsIdentity = new ClaimsIdentity(new List<Claim>
                //{
                //new Claim(ClaimTypes.Email,  user.UserName),
                //new Claim(ClaimTypes.GivenName,  user.Email),
                //new Claim(ClaimTypes.Sid,  user.Id),
                //new Claim(JwtRegisteredClaimNames.Aud, "chargeatfriends"),
                //new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
                //}, "Cookies");




                //Set issued at date
                //DateTime issuedAt = DateTime.UtcNow;

                //RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                //string publicPrivateKey = System.IO.File.ReadAllText("PublicPrivateKey.xml");
                //provider.FromXmlString(publicPrivateKey);
                //RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(provider);


                ////const string sec = HostConfig.SecurityKey;
                //const string sec = "4gSd0AsIoPvyD3PsXYNrP2XnVpIYCLLL";
                //var now = DateTime.UtcNow;
                //var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));
                //var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                //DateTime expires = DateTime.UtcNow.AddMonths(5);

                ////create the jwt
                //var token = (JwtSecurityToken)tokenHandler.CreateJwtSecurityToken(
                //            issuer: "chargeatfriends",
                //            audience: "https://localhost:7275",
                //            subject: claimsIdentity,
                //            notBefore: issuedAt,
                //            signingCredentials: signingCredentials,
                //            expires: expires);

                //var AccessToken = tokenHandler.WriteToken(token);

                //result.TokenType = "Access_Token";
                //result.AccessToken = AccessToken;
                #endregion

                DateTime expires = DateTime.UtcNow.AddMonths(5);

                var claims = new List<Claim>
                {
                    //new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Expiration, expires.ToString()),
                    //new Claim(ClaimTypes.Email,  user.UserName),
                    new Claim(ClaimTypes.GivenName,  user.Email),
                    //new Claim(ClaimTypes.Sid,  user.Id),
                    //new Claim(JwtRegisteredClaimNames.Aud, "chargeatfriends"),
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
                };
                if (user.Email.ToLower().EndsWith("@chargeatfriends.com"))
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }

                var accessToken = _tokenService.GenerateAccessToken(claims);
                var refreshToken = _tokenService.GenerateRefreshToken();


                #region save refresh token and access token in OAuthToken table
                var idptoken = new OAuthTokenEntity
                {
                    ClientId = "ChargeatFriends",
                    CreationDate = DateTime.Now,
                    ReferenceId = Guid.NewGuid().ToString(),
                    Status = Constants.Statuses.Valid,
                    Token = accessToken,
                    TokenType = Constants.TokenTypes.JWTIdentityToken,
                    ExpirationDate = expires,
                    SubjectId = user.Id,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiryTime = expires,
                    UserName = user.UserName,
                };
                _context.OAuthTokens.Add(idptoken);
                _context.SaveChanges();

                #endregion

                return Content(System.Text.Json.JsonSerializer.Serialize(new SSOResponse.CAFResponse
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = accessToken,
                    GuID = user.Id,
                    CreationDate = DateTime.Now,
                    Status = Constants.Statuses.Valid,
                    TokenType = Constants.TokenTypes.JWTIdentityToken,
                    RefreshToken = refreshToken,
                    ExpirationDate = expires,
                    RefreshTokenExpiryTime = expires,

                }), APPLICATION_JSON);

                //return Ok(new { Token = AccessToken, userDetails = user });
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
        {
            return ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateLifetime = false // we check expired tokens here
            });
        }
        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            try
            {
                JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

                var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        #region two factor authenication code
        /// <summary>
        /// Two factor authentication logic - 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [EnableCors("OpenCORSPolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginTwoStep(string email, string returnUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendEmailTwoFactorCode(user.Email, token);

            return View();
        }
        [EnableCors("OpenCORSPolicy")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginTwoStep(TwoFactor twoFactor, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(twoFactor.TwoFactorCode);
            }

            var result = await _signInManager.TwoFactorSignInAsync("Email", twoFactor.TwoFactorCode, false, false);
            if (result.Succeeded)
            {
                return Redirect(returnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError("", "Ungültiger Anmeldeversuch");
                return View();
            }
        }


        #endregion


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model.RecoveryCode);
            }

            var result = await _signInManager.TwoFactorSignInAsync(_userManager.Options.Tokens.AuthenticatorTokenProvider, model.RecoveryCode, false, false);
            if (result.Succeeded)
            {
                return Redirect(returnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError("", "Ungültiger Anmeldeversuch");
                return View();
            }
        }

        public IActionResult Lockout()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion

        [HttpPost]
        public IActionResult ChangeLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    }
            );

            return LocalRedirect(returnUrl);
        }

    }
}
