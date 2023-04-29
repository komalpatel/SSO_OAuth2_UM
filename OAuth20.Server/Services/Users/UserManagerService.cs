using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using OAuth20.Server.Models;
using OAuth20.Server.Models.AccountViewModels;
using OAuth20.Server.Models.ManageViewModels;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using OAuth20.Server.SSORequest;
using OAuth20.Server.SSOResponse;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth20.Server.Services.Users
{
    public class UserManagerService : IUserManagerService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<UserManagerService> _logger;

        public UserManagerService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ILogger<UserManagerService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginUserAsync(LoginRequest request)
        {
            var validationResult = validateLoginRequest(request);

            if (!validationResult)
            {
                _logger.LogInformation("validation for login process is failed {request}", request);
                return new LoginResponse { Error = "login process is failed" };
            }

            AppUser user = null;

            user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null && request.UserName.Contains("@"))
                user = await _userManager.FindByEmailAsync(request.UserName);

            if (user == null)
            {
                _logger.LogInformation("creditioanl {userName}", request.UserName);
                return new LoginResponse { Error = "No user has this creditioanl" };
            }

            await _signInManager.SignOutAsync();


            Microsoft.AspNetCore.Identity.SignInResult loginResult = await _signInManager
                .PasswordSignInAsync(user, request.Password, false, false);

            if (loginResult.Succeeded)
            {
                return new LoginResponse { Succeeded = true };
            }

            return new LoginResponse { Succeeded = false, Error = "Login is not Succeeded" };

        }

        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var validationResult = validateCreateUserRequest(request);

            if (!validationResult)
            {
                _logger.LogInformation("The create user request is failed please check your input {request}", request);
                return new CreateUserResponse { Error = "The create user request is failed please check your input" };
            }


            var user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                PhoneNumber = request.PhoneNumber,
                TwoFactorEnabled = false,
                PasswordHash = request.Password,
                LastLoginDate = DateTime.Now.ToString(),
            };


            var createUserResult = await _userManager.CreateAsync(user, request.Password);

            if (createUserResult.Succeeded)
                return new CreateUserResponse { Succeeded = true };

            return new CreateUserResponse { Error = createUserResult.Errors.Select(x => x.Description).FirstOrDefault() };



        }

        public async Task<CreateProfileResponse> CreateUserProfileAsync(CreateProfileRequest request)
        {
           
            var validationResult = validateCreateProfileUserRequest(request);

            if (!validationResult)
            {
                _logger.LogInformation("The create user request is failed please check your input {request}", request);
                return new CreateProfileResponse { Error = "The create user request is failed please check your input" };
            }
            var user = await _userManager.FindByNameAsync(request.UserName);


            if (request.FirstName != null)
            {
                user.FirstName = request.FirstName;

            }
            else
            {
                user.FirstName = user.FirstName;
            }
            if (request.LastName != null)
            {
                user.LastName = request.LastName;

            }
            else
            {
                user.LastName = user.LastName;
            }

            if (request.Email != null)
            {
                user.Email = request.Email;

            }
            else
            {
                user.Email = user.Email;
            }

            if (request.PhoneNumber != null)
            {
                user.PhoneNumber = request.PhoneNumber;

            }
            else
            {
                user.PhoneNumber = user.PhoneNumber;
            }

        
            //user.FirstName = request.FirstName;
            //user.PhoneNumber = request.PhoneNumber;
            //user.LastName = request.LastName;
            user.LastLoginDate = DateTime.Now.ToString();

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return new CreateProfileResponse { Succeeded = true };
                

            return new CreateProfileResponse { Error = result.Errors.Select(x => x.Description).FirstOrDefault() };

        }

        public async Task<OpenIdConnectLoginResponse> LoginUserByOpenIdAsync(OpenIdConnectLoginRequest request)
        {
            bool validationResult = validateOpenIdLoginRequest(request);
            if (!validationResult)
            {
                _logger.LogInformation("login process is failed for request: {request}", request);
                return new OpenIdConnectLoginResponse { Error = "The login process is failed" };
            }

            AppUser user = null;

            user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null && request.UserName.Contains("@"))
                user = await _userManager.FindByEmailAsync(request.UserName);

            if (user == null)
            {
                _logger.LogInformation("creditioanl {userName}", request.UserName);
                return new OpenIdConnectLoginResponse { Error = "No user has this creditioanl" };
            }

            await _signInManager.SignOutAsync();


            Microsoft.AspNetCore.Identity.SignInResult loginResult = await _signInManager
                .PasswordSignInAsync(user, request.Password, false, false);

            if (loginResult.Succeeded)
            {
                return new OpenIdConnectLoginResponse { Succeeded = true, AppUser = user };
            }

            return new OpenIdConnectLoginResponse { Succeeded = false, Error = "Login is not Succeeded" };
        }

        #region Helper Functions
        private bool validateLoginRequest(LoginRequest request)
        {
            if (request.UserName == null || request.Password == null)
                return false;

            if (request.Password.Length < 8)
                return false;

            return true;
        }

        private bool validateOpenIdLoginRequest(OpenIdConnectLoginRequest request)
        {
            if (request.UserName == null || request.Password == null)
                return false;
            return true;
        }

        private bool validateCreateUserRequest(CreateUserRequest request)
        {
            if (request.Password == null || request.Email == null)
                return false;
            return true;
        }
        private bool validateCreateProfileUserRequest(CreateProfileRequest request)
        {
            if (request.UserName == null)
                return false;
            return true;
        }
        #endregion
    }
}
