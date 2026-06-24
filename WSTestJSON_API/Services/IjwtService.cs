using WSTestJSON_API.Models;

namespace WSTestJSON_API.Services
{
    public interface IjwtService
    {
        string GenerarToken(Usuarios usuario);
    }
}
