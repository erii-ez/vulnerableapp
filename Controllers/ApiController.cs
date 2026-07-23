using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using VulnerableApp.Data;
using System.Diagnostics; // Necesario para Stopwatch

namespace VulnerableApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ApiController> _logger; // 1. Inyectamos el logger

        public ApiController(AppDbContext db, ILogger<ApiController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var sessionUser = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio ApiController.GetUser");

            // Registramos los parámetros de entrada requeridos
            _logger.LogInformation("Usuario: {User} IP: {IP} solicitando datos del usuario ID: {RequestedId}", sessionUser, ip, id);

            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                if (!currentUserId.HasValue)
                {
                    _logger.LogWarning("Acceso no autorizado a la API. Se rechazó petición a {IP}", ip);

                    stopwatch.Stop();
                    _logger.LogInformation("Fin ApiController.GetUser. Resultado: Unauthorized. Tiempo: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                    return Unauthorized();
                }

                if (id != currentUserId.Value)
                {
                    _logger.LogWarning("El usuario {User} intentó acceder a los datos de otro usuario distinto (ID solicitado: {RequestedId})", sessionUser, id);

                    stopwatch.Stop();
                    _logger.LogInformation("Fin ApiController.GetUser. Resultado: Forbid. Tiempo: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                    return Forbid();
                }

                var user = _db.Users.Find(id);
                if (user == null)
                {
                    _logger.LogWarning("El usuario {User} solicitó el ID: {RequestedId} el cual no existe en la base de datos.", sessionUser, id);

                    stopwatch.Stop();
                    _logger.LogInformation("Fin ApiController.GetUser. Resultado: NotFound. Tiempo: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                    return NotFound();
                }

                stopwatch.Stop();
                _logger.LogInformation("Fin ApiController.GetUser. Resultado: OK. Tiempo: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                return Ok(new { user.Id, user.Username, user.Email });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error crítico en ApiController.GetUser consultando ID {RequestedId}. Tiempo: {ElapsedMilliseconds} ms", id, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var sessionUser = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio ApiController.GetAllUsers");
            _logger.LogInformation("Usuario: {User} IP: {IP} solicitando el listado general de usuarios", sessionUser, ip);

            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                if (!currentUserId.HasValue)
                {
                    _logger.LogWarning("Acceso no autorizado a la lista de usuarios. IP: {IP}", ip);

                    stopwatch.Stop();
                    _logger.LogInformation("Fin ApiController.GetAllUsers. Resultado: Unauthorized. Tiempo: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                    return Unauthorized();
                }

                var safeUsers = _db.Users.Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email
                }).ToList();

                stopwatch.Stop();
                _logger.LogInformation("Fin ApiController.GetAllUsers. Resultado: OK. Cantidad devuelta: {TotalUsers}. Tiempo: {ElapsedMilliseconds} ms", safeUsers.Count, stopwatch.ElapsedMilliseconds);

                return Ok(safeUsers);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error crítico en ApiController.GetAllUsers. Tiempo: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}