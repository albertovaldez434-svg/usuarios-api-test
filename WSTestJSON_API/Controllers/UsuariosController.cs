using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
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


        //obtener tdos los usuarios
        // GET: api/Usuarios
        [HttpGet("getusuarios")]
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
                var loginData = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => request.Usuario == u.NombreUsuario);
                if (loginData == null)
                    return NotFound("Datos incorrectos");

                if (request.Password != loginData?.Password)
                    return Unauthorized("Datos incorrectos");

                //generar jwt
                var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var sign = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.UserData, request.Usuario)
                };

                var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: sign);
                var jwtHandlder = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    access_token = jwtHandlder,
                    token_type = "bearer",
                    user_id = loginData.IdUser,
                    user_name = loginData.NombreUsuario
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error al obtener login");
                return StatusCode(500, "Error Interno del servidor");
            }
        }

        //POST: api/usuarios/registrar
        [HttpPost("registrar")]
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
        [HttpPut("editar/{id}")]
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


    }
}
