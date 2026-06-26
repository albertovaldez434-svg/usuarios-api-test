using WSTestJSON_API.Data;

namespace WSTestJSON_API.Services
{
    public class restorePsw : IRestorePsw
    {
        private readonly APIDbContext _context;

        public restorePsw(APIDbContext dbcontext)
        {
            _context = dbcontext;
        }

        public async Task SetNewPassword(string newPsw)
        {


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
