namespace WSTestJSON_API.DTOs
{
    public class UsuariosDTO
    {
        public int IdUser { get; set; } 
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? IdRol { get; set; }
    }
}
