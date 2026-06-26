using WSTestJSON_API.Data;
using WSTestJSON_API.Models;

namespace WSTestJSON_API.Services
{
    public class UploadImgService : IUploadImgService
    {
        private readonly APIDbContext _context;
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;

        public UploadImgService (APIDbContext dbcontext, IHttpClientFactory httpClient, IConfiguration config)
        {
            _context = dbcontext;
            _httpClient = httpClient;
            _configuration = config;
        }

        public async Task<ImagenesUsuarios> UploadImage(IFormFile file, int IdUser)
        {
            // extensión
            var extension = Path.GetExtension(file.FileName);

            // nombre único
            var fileName = "avatar.webp";

            // path en storage
            var pathArchivo = $"{IdUser}/{fileName}";

            // URL pública
            var publicUrl = $"https://hdsarnayyialwynbafhw.supabase.co/storage/v1/object/public/avatars/{pathArchivo}";

            // =========================
            // UPLOAD REAL A SUPABASE
            // =========================

            using var client = _httpClient.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    _configuration["Supabase:ServiceRoleKey"]
                );

            // opcional: reemplazar si existe
            client.DefaultRequestHeaders.Add(
                "x-upsert",
                "true"
            );

            using var content =
                new StreamContent(file.OpenReadStream());

            content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(
                    file.ContentType
                );

            //var response = await client.PostAsync(
            //    $"https://hdsarnayyialwynbafhw.supabase.co/storage/v1/object/avatars/{pathArchivo}",
            //    content
            //);

            //var responseText = await response.Content.ReadAsStringAsync();

            //if (!response.IsSuccessStatusCode)
            //{
            //    return BadRequest(responseText);
            //}

            // =========================
            // UPLOAD metadata
            // =========================

            var imagen = new ImagenesUsuarios
            {
                Nombre = fileName,
                PathArchivo = pathArchivo,
                URLPublica = publicUrl,
                MimeType = file.ContentType,
                Extension = extension,
                IdUser = IdUser
            };

            _context.ImagenesUsuarios.Add(imagen);

            await _context.SaveChangesAsync();
            return imagen;
        }
    }
}
