using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using System.Data;
using System.Data.Common;
using Dapper;
using WSTestJSON_API.Models;

namespace WSTestJSON_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DapperController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DapperController> logger;

        public DapperController(IConfiguration _Config, ILogger<DapperController> _logger)
        {
            _config = _Config;
            logger = _logger;
        }

        private IDbConnection MyDBConnection() => new SqlConnection(_config.GetConnectionString("WSTestJSON_APIContext"));

        //GET /api/dapper
        [HttpGet]
        public async Task<IActionResult> getAllUsers()
        {
            try
            {   using var db = MyDBConnection();
                string sqlCommand = "select top 20 * from Users";
                var users = await db.QueryAsync<Usuarios>(sqlCommand);
                return Ok(users);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, "Error Interno del servidor");
            }
        }

        //http get{id}
    }
}
