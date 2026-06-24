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

        public DbSet<TareasUsuario> TareasUsuario { get; set; }

        public DbSet<ImagenesUsuarios> ImagenesUsuarios { get; set; }
    }
}