using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Configuration;
using System.Security.Claims;
using WSTestJSON_API.Data;
using WSTestJSON_API.Models;

namespace WSTestJSON_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly APIDbContext _context;
        private readonly ILogger<TasksController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;


        public TasksController(APIDbContext context, ILogger<TasksController> logger, IConfiguration configuration, IHttpClientFactory httpclient)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpclient;
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
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            var tarea = await _context.TareasUsuario.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }

            try
            {
                _context.TareasUsuario.Remove(tarea);
                await _context.SaveChangesAsync();
                return Ok(tarea);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tarea");
                return StatusCode(500, "Error Interno del servidor");
            }
        }
    }
}
