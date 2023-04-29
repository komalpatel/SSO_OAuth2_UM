using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using OAuth20.Server.Models;
using OAuth20.Server.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OAuth20.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<AppUser> _signInManager;
		private readonly UserManager<AppUser> _userManager;
		private readonly ILogger<RegisterModel> _logger;
		//private readonly IEmailSenderExtended _emailSender;
		
		private static readonly HttpClient _httpClient = new HttpClient();
		private static string reCaptureSecret = StaticData.Configuration["reCaptureSecret"];

		//private readonly IEmailSender _emailSender;

		public RegisterModel(
			UserManager<AppUser> userManager,
			SignInManager<AppUser> signInManager,
			ILogger<RegisterModel> logger
			//IEmailSender emailSender
			//IEmailSenderExtended emailSender
			)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
			//_emailSender = emailSender;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public string ReturnUrl { get; set; }

		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		public class InputModel
		{
			[Required]
			[EmailAddress]
			[Display(Name = "Email")]
			public string Email { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "Das {0} muss mindestens {2} und maximal {1} Zeichen lang sein.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "Passwort")]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Passwort bestätigen")]
			[Compare("Password", ErrorMessage = "Das Passwort und seine Bestätigung stimmen nicht überein.")]
			public string ConfirmPassword { get; set; }
		}

		public async Task OnGetAsync(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl = returnUrl ?? Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			string googleReCaptchResponse = HttpContext.Request.Form["g-recaptcha-response"];
			System.Net.IPAddress clientIp = HttpContext.Connection.RemoteIpAddress;
			bool reCaptureSuccess = await HttpPostAsync(googleReCaptchResponse, clientIp);
			if (ModelState.IsValid && reCaptureSuccess)
			{
				var partner = Request.Cookies["partner"];
				var user = new AppUser { UserName = Input.Email, Email = Input.Email };
				var result = await _userManager.CreateAsync(user, Input.Password);
				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");
					await TableStorageHelper.Instance.CreateCustomerEntity(Input.Email, Input.Password, Input.Email, partner);
					Backend.UpdateCustomer(Input.Email);

					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
						protocol: Request.Scheme);
					try
					{

						//await _emailSender.SendEmailAsync(
						//	Input.Email, "Bitte bestätige Deine E-Mail",
						//	$"<h1>Lieber Charge@Friends Nutzer,</h1><br/><p>Um Deinen Account zu bestätigen bitte <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>hier klicken</a>.</p><br/>Herzliche Grüße,<br/>das Charge@Friends Team");
					}
					catch (Exception err)
					{
						Debug.WriteLine(err.Message);
					}

					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
					}
					else
					{
						await _signInManager.SignInAsync(user, isPersistent: false);
						return LocalRedirect(returnUrl);
					}
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
		private string googleReCaptureUrl = "https://www.google.com/recaptcha/api/siteverify";
		private async Task<bool> HttpPostAsync(string response, System.Net.IPAddress clientIp)
		{
			var content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("secret", reCaptureSecret),
				new KeyValuePair<string, string>("response", response),
				new KeyValuePair<string, string>("remoteip", clientIp.ToString())

			});
			var result = await _httpClient.PostAsync(googleReCaptureUrl, content);
			string resultContent = await result.Content.ReadAsStringAsync();
			ReCaptureResult reCaptureResult = JsonConvert.DeserializeObject<ReCaptureResult>(resultContent);

			return reCaptureResult.success;
		}
		public class ReCaptureResult
		{
			public bool success;
			public string challenge_ts;
			public string apk_package_name;
			[JsonProperty("error-codes")]
			public string[] errorCodes;
		}
	}
}
