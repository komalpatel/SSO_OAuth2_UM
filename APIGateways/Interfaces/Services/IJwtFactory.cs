using System.Threading.Tasks;
using APIGateways.Interfaces.DTO;

namespace APIGateways.Interfaces.Services
{
    public interface IJwtFactory
    {
        Task<AccessToken> GenerateEncodedToken(string id, string userName);
    }
}
