namespace WSTestJSON_API.Models
{
    public class ImagenesUsuarios
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string PathArchivo { get; set; }
        public string URLPublica { get; set; }
        public string MimeType { get; set; }
        public string Extension { get; set; }
        public int IdUser { get; set; }
    }
}
