using WSTestJSON_API.Models;

namespace WSTestJSON_API.Services
{
    public interface IAuthService
    {
        Task<bool> ValidateHashPsw(Usuarios usuario, string password);
    }
}
