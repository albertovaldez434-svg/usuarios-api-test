using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Security.Claims;
using WSTestJSON_API.Data;
using WSTestJSON_API.DTOs;
using WSTestJSON_API.Models;
using WSTestJSON_API.Services;

namespace WSTestJSON_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ILogger<UsuariosController> _logger;
        private readonly APIDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpFactory;
        private readonly IjwtService _jwtService;
        private readonly IAuthService _authService;
        private readonly IUploadImgService _uploadImgService;

        public UsuariosController(APIDbContext context, ILogger<UsuariosController> logger, IConfiguration config, IHttpClientFactory httpFact, IjwtService ijwtSrvice, IAuthService authService, IUploadImgService uploadImgService)
        {
            _context = context;
            _logger = logger;
            _configuration = config;
            _httpFactory = httpFact;
            _jwtService = ijwtSrvice;
            _authService = authService;
            _uploadImgService = uploadImgService;

        }

        // test de conexion
#if DEBUG
        [HttpGet("test-db")]
        public async Task<IActionResult> TestDb()
        {
            try
            {
                using var connection = new NpgsqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                await connection.OpenAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
#endif


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

                bool passwordInvalid = await _authService.ValidateHashPsw(loginData, request.Password);

                if (!passwordInvalid)
                    return Unauthorized();

                // buscar imagen
                var imagen = await _context.ImagenesUsuarios.Where(img => img.IdUser == loginData.IdUser).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                var avatarUrl = imagen?.URLPublica;

                var jwtHandler = _jwtService.GenerarToken(loginData);

                int? idRol = loginData.IdRol;

                var response = new LoginResponseDTO
                {
                    AccessToken = jwtHandler,
                    IdUser = loginData.IdUser,
                    IdRol = idRol,
                    Nombre = loginData.Nombre,
                    Apellidos = loginData.Apellidos,
                    Email = loginData.Email,
                    Avatar = avatarUrl
                };

                return Ok(response);
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

                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

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
        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarios(int id)
        {
            var currentUsrId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (currentUsrId == id)
            {
                return Forbid();
            }

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
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (currentUserId != id)
            {
                return Forbid();
            }

            try
            {
                var tareas = await _context.TareasUsuario.Where(tasks => tasks.IdUser == id).ToListAsync();
                if (!tareas.Any())
                {
                    return NoContent();
                }
                return Ok(tareas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar tareas");
                return StatusCode(500, "Error Interno del servidor");
            }
        }

        [Authorize]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateTarea([FromBody] TareasUsuario tareasUsuario)
        {
            var tareas = await _context.TareasUsuario.FirstOrDefaultAsync(tasks => tasks.Id == tareasUsuario.Id);

            if (tareas == null)
            {
                return NotFound();
            }
            try
            {
                tareas.Title = tareasUsuario.Title;
                tareas.Description = tareasUsuario.Description;
                tareas.Status = tareasUsuario.Status;

                await _context.SaveChangesAsync();
                return Ok(tareas);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tarea");
                return StatusCode(500, "Error Interno del servidor");
            }



        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddTarea([FromBody] TareasUsuario nuevatarea)
        {
            if (nuevatarea == null)
            {
                return BadRequest();
            }

            try
            {
                await _context.AddAsync(nuevatarea);
                await _context.SaveChangesAsync();
                return Ok(nuevatarea);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar nueva tarea");
                return StatusCode(500, "Error Interno del servidor");
            }
        }


        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> CargarImagen(IFormFile file, [FromForm] int IdUser)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }

            if (file.Length > 5_000_000)
            {
                return BadRequest("Archivo demasiado grande");
            }

            var allowedTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!allowedTypes.Contains(file.ContentType))
            {
                return BadRequest();
            }

            //comienza proceso guardado
            try
            {
                var imagen = await _uploadImgService.UploadImage(file, IdUser);

                return Ok(imagen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar imagen");
                return StatusCode(500, "Error Interno del servidor");
            }


        }


    }
}
