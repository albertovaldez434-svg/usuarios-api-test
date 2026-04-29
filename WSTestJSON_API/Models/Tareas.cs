using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;

namespace WSTestJSON_API.Models
{
    public class TareasUsuario
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int IdUser { get; set; }

    }
}
