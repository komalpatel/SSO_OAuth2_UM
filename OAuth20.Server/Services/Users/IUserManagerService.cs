using OAuth20.Server.Models.AccountViewModels;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using OAuth20.Server.SSORequest;
using OAuth20.Server.SSOResponse;
using System.Threading.Tasks;

namespace OAuth20.Server.Services.Users
{
    public interface IUserManagerService
    {
        Task<LoginResponse> LoginUserAsync(LoginRequest request);
        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
        Task<OpenIdConnectLoginResponse> LoginUserByOpenIdAsync(OpenIdConnectLoginRequest request);


        Task<CreateProfileResponse> CreateUserProfileAsync(CreateProfileRequest request);
    }
}
