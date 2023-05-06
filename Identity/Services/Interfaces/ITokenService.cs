using Identity.Dtos;
using Identity.Models;

namespace Identity.Services.Interfaces
{
    public interface ITokenService
    {
        Task<Token> GetUserTokenAsync(UserTokenDto userToken);
    }
}
