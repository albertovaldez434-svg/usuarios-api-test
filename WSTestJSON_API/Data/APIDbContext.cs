using Microsoft.EntityFrameworkCore;
using WSTestJSON_API.Models;

namespace WSTestJSON_API.Data
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Usuarios> Usuarios { get; set; }
    }

    public class APIContext : DbContext
    {
        public APIContext(DbContextOptions<APIContext> dbCont) : base(dbCont)
        {

        }
    }
}