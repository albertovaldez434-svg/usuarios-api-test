using System.ComponentModel.DataAnnotations;

namespace WSTestJSON_API.Models
{
    public class Usuarios
    {
        [Key]
        public int IdUser { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public int? IdRol { get; set; } 
        public string? NombreUsuario { get; set; }
        public string? Password { get; set; }
    }


}
