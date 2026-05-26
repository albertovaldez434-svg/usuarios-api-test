using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WSTestJSON_API.Data;
using WSTestJSON_API.Models;

namespace WSTestJSON_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ILogger<UsuariosController> _logger;
        private readonly APIDbContext _context;
        private readonly IConfiguration _configuration;

        public UsuariosController(APIDbContext context, ILogger<UsuariosController> logger, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _configuration = config;
        }

        [HttpGet("test-db")]
        public async Task<IActionResult> TestDb()
        {
            try
            {
                using var connection = new NpgsqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                await connection.OpenAsync();

                return Ok("Conexion exitosa");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //obtener todos los usuarios
        // GET: api/Usuarios
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuariosList()
        {
            try
            {
                var usuarios = await _context.Usuarios.AsNoTracking().ToListAsync();

                if (!usuarios.Any())
                {
                    return NoContent();
                }

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Usuarios");
                return StatusCode(500, "Error interno del servidor");
            }

        }

        // hacer un login
        // get: api/Usuarios/Login
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            try
            {
                var loginData = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => request.Email == u.Email);
                if (loginData == null)
                    return NotFound("Datos incorrectos");

                if (request.Password != loginData?.Password)
                    return Unauthorized("Datos incorrectos");

                // buscar imagen
                var imagen = await _context.ImagenesUsuarios.Where(img => img.IdUser == loginData.IdUser).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                var avatarUrl = imagen?.URLPublica;

                //generar jwt
                var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var sign = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.UserData, request.Email)
                };

                var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: sign);
                var jwtHandlder = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    access_token = jwtHandlder,
                    token_type = "bearer",
                    idUser = loginData.IdUser,
                    idRol = loginData.IdRol,
                    userInfo = loginData,
                    avatar = avatarUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error al obtener login");
                return StatusCode(500, "Error Interno del servidor");
            }
        }

        //POST: api/usuarios/registrar
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RegistrarUsuario([FromBody] Usuarios usuario)
        {
            try
            {
                var usrData = await _context.Usuarios.FirstOrDefaultAsync(usr => usr.Email == usuario.Email);
                if (usrData != null)
                {
                    return BadRequest("Ya existe un usuario con estos datos");
                }

                await _context.AddAsync(usuario);
                await _context.SaveChangesAsync();
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error al obtener login");
                return StatusCode(500, "Error Interno del servidor");
            }
        }

        // obtener un usuario, pendiente estoy pensando como hacerlo mejor
        // GET: api/Usuarios/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Usuarios>> GetUsuarios(int id)
        //{
        //    var usuarios = await _context.Usuarios.FindAsync(id);

        //    if (usuarios == null)
        //    {
        //        return NotFound("No se encontro el usuario requerido");
        //    }

        //    return usuarios;
        //}

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarUsuario(int id, [FromBody] Usuarios usuario)
        {
            if (id != usuario.IdUser) return BadRequest();

            try
            {
                var userData = await _context.Usuarios.FirstOrDefaultAsync(usr => usr.IdUser == usuario.IdUser);
                if (userData == null)
                {
                    return BadRequest("No se encontro el usuario seleccionado");
                }

                userData.Nombre = usuario.Nombre;
                userData.Apellidos = usuario.Apellidos;
                userData.Email = usuario.Email;
                userData.Telefono = usuario.Telefono;

                await _context.SaveChangesAsync();
                return Ok(userData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error al editar usuario");
                return StatusCode(500, "Error Interno del servidor");
            }
        }

        // eliminar un usuario
        // DELETE: api/Usuarios/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarios(int id)
        {

            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario");
                return StatusCode(500, "Error Interno del servidor");
            }

        }

        [Authorize]
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<IEnumerable<TareasUsuario>>> GetTareas(int id)
        {
            var tareas = await _context.TareasUsuario.Where(tasks => tasks.IdUser == id).ToListAsync();
            if (!tareas.Any())
            {
                return NoContent();
            }
            return Ok(tareas);
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateTarea([FromBody] TareasUsuario tareasUsuario)
        {
            var tareas = await _context.TareasUsuario.Where(tasks => tasks.Id == tareasUsuario.Id).FirstAsync();
            if (tareas == null)
            {
                return NoContent();
            }

            tareas.Title = tareasUsuario.Title;
            tareas.Description = tareasUsuario.Description;
            tareas.Status = tareasUsuario.Status;

            await _context.SaveChangesAsync();
            return Ok(tareas);
        }


        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> CargarImagen(IFormFile file, [FromForm] int IdUser)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }

            // extensión
            var extension = Path.GetExtension(file.FileName);

            // nombre único
            var fileName = $"{Guid.NewGuid()}.webp";

            // path en storage
            var pathArchivo = $"{IdUser}/{fileName}";

            // URL pública
            var publicUrl = $"https://hdsarnayyialwynbafhw.supabase.co/storage/v1/object/public/avatars/{pathArchivo}";

            // =========================
            // UPLOAD REAL A SUPABASE
            // =========================

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Imhkc2FybmF5eWlhbHd5bmJhZmh3Iiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc3OTMyMTU1MCwiZXhwIjoyMDk0ODk3NTUwfQ.xTgSKQ6WZ6khv7mDcau3ec1ci9J27CMpno3xrz_i0DI"
                );

            // opcional: reemplazar si existe
            //client.DefaultRequestHeaders.Add(
            //    "x-upsert",
            //    "true"
            //);

            using var content =
                new StreamContent(file.OpenReadStream());

            content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(
                    file.ContentType
                );

            var response = await client.PostAsync(
                $"https://hdsarnayyialwynbafhw.supabase.co/storage/v1/object/avatars/{pathArchivo}",
                content
            );

            var responseText =
                await response.Content.ReadAsStringAsync();

            Console.WriteLine(response.StatusCode);
            Console.WriteLine(responseText);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(responseText);
            }

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
            return Ok(imagen);
        }


    }
}
