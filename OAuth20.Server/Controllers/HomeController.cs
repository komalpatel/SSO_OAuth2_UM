using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using OAuth20.Server.Models;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.Services;
using OAuth20.Server.Services.CodeServce;
using OAuth20.Server.Services.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth20.Server.Controllers
{
    public class HomeController : Controller
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizeResultService _authorizeResultService;
        private readonly ICodeStoreService _codeStoreService;
        private readonly IUserManagerService _userManagerService;
        private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory;

        public HomeController(IHttpContextAccessor httpContextAccessor, IAuthorizeResultService authorizeResultService,
            ICodeStoreService codeStoreService, IUserManagerService userManagerService,
            IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _authorizeResultService = authorizeResultService;
            _codeStoreService = codeStoreService;
            _userManagerService = userManagerService;
            _userClaimsPrincipalFactory= userClaimsPrincipalFactory;
        }

        //[Route("[controller]/[action]/")]
        //[Route("[controller]/")]
        //[Route("/")]
        //[Route("/{id?}")]
        //[Authorize]
        ////[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> Index()
        //{
        //	if (User.Identity.IsAuthenticated)
        //	{


        //		string url = HttpContext.Request.Path;
        //		string[] parts = url.Split('/');
        //		Guid guid = new Guid();
        //		if (Guid.TryParseExact(parts.Last(), "D", out guid))
        //		{
        //			if (HttpContext.Request.Host.ToString().ToLower().Contains("app.chargeatfriends.com"))
        //			{
        //				return RedirectToAction("GotoStore", "ConnectMobile");
        //			}
        //		}
        //		if (HttpContext.Request.Host.ToString().ToLower().Contains("app.chargeatfriends.com"))
        //		{
        //			return Redirect("https://www.chargeatfriends.com");
        //		}

        //		ViewBag.IsFirst = false;
        //		if (!string.IsNullOrEmpty(User.Identity.Name))
        //		{
        //                  ViewBag.IsFirst = true;
        //                  //CustomerEntity customerEntity = await TableStorageHelper.Instance.GetCustomerEntityByEmail(User.Identity.Name);
        //                  //if (customerEntity != null && string.IsNullOrEmpty(customerEntity.StripeId))
        //                  //{
        //                  //	ViewBag.IsFirst = true;

        //                  //}
        //              }
        //	}
        //	return View();
        //}
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
 
        public IActionResult Authorize(AuthorizationRequest authorizationRequest)
        {
            var result = _authorizeResultService.AuthorizeRequest(_httpContextAccessor, authorizationRequest);

            if (result.HasError)
                return RedirectToAction("Error", new { error = result.Error });

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var updateCodeResult = _codeStoreService.UpdatedClientDataByCode(result.Code, 
                    _httpContextAccessor.HttpContext.User, result.RequestedScopes);
                if (updateCodeResult != null)
                {
                    result.RedirectUri = result.RedirectUri + "&code=" + result.Code;
                    return Redirect(result.RedirectUri);
                }
                else
                {
                    return RedirectToAction("Error", new { error = "invalid_request" });
                }

            }

            var loginModel = new OpenIdConnectLoginRequest
            {
                RedirectUri = result.RedirectUri,
                Code = result.Code,
                RequestedScopes = result.RequestedScopes,
            };
            return View("Login", loginModel);
        }

        [Route("[controller]/[action]/")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("[controller]/[action]/")]
        [HttpPost]
        public async Task<IActionResult> Login(OpenIdConnectLoginRequest loginRequest)
        {
            // here I have to check if the username and passowrd is correct
            // and I will show you how to integrate the ASP.NET Core Identity
            // With our framework

            if (!loginRequest.IsValid())
                return RedirectToAction("Error", new { error = "invalid_request" });
            var userLoginResult = await _userManagerService.LoginUserByOpenIdAsync(loginRequest);

            if (userLoginResult.Succeeded)
            {
                var claimsPrincipals = await _userClaimsPrincipalFactory.CreateAsync(userLoginResult.AppUser);
                var result = _codeStoreService.UpdatedClientDataByCode(loginRequest.Code,
                    claimsPrincipals, loginRequest.RequestedScopes);
                if (result != null)
                {
                    loginRequest.RedirectUri = loginRequest.RedirectUri + "&code=" + loginRequest.Code;
                    return Redirect(loginRequest.RedirectUri);
                }
            }

            return RedirectToAction("Error", new { error = "invalid_request" });
        }
        [HttpPost]
        public JsonResult Token(TokenRequest tokenRequest)
        {
            var result = _authorizeResultService.GenerateToken(tokenRequest);

            if (result.HasError)
                return Json(new
                {
                    error = result.Error,
                    error_description = result.ErrorDescription
                });

            return Json(result);
        }

        public IActionResult Error(string error)
        {
            return View(error);
        }

    

        [Route("[controller]/[action]/")]
       
        [HttpPost]
        public IActionResult Getclaims()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
        [HttpPost]
        public IActionResult ChangeLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddMinutes(7)
                    }
            );

            return LocalRedirect(returnUrl);
        }

    }
}
