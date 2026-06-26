using BCrypt.Net;
using WSTestJSON_API.Data;
using WSTestJSON_API.Models;

namespace WSTestJSON_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly APIDbContext _dbContext;

        public AuthService(APIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ValidateHashPsw(Usuarios usuario, string password)
        {
            if(!EsHashBCrypt(usuario.Password))
            {
                if (usuario.Password == password)
                {
                    usuario.Password = BCrypt.Net.BCrypt.HashPassword(password);

                    await _dbContext.SaveChangesAsync();

                    return true;
                }

                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, usuario.Password);
        }

        private bool EsHashBCrypt(string password)
        {
            return password.StartsWith("$2a$") ||
                   password.StartsWith("$2b$") ||
                   password.StartsWith("$2x$") ||
                   password.StartsWith("$2y$");
        }

    }


}
