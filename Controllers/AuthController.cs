using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using VulnerableApp.Data;
using System.Diagnostics; // Necesario para el Stopwatch

namespace VulnerableApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AuthController> _logger; // Inyectamos el Logger

        public AuthController(AppDbContext db, ILogger<AuthController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var sessionUser = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio AuthController.Login (GET)");
            _logger.LogInformation("Usuario: {User} IP: {IP} solicitando la vista de inicio de sesión", sessionUser, ip);

            try
            {
                var result = View();

                stopwatch.Stop();
                _logger.LogInformation("Fin AuthController.Login (GET). Tiempo de ejecución: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error crítico en AuthController.Login (GET). Tiempo: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var sessionUser = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio AuthController.Login (POST)");

            _logger.LogInformation("Usuario: {User} IP: {IP}. Intentando autenticar al usuario: {LoginUsername}", sessionUser, ip, username);

            try
            {
                var user = _db.Users.FirstOrDefault(u => u.Username == username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning("Evento de autenticación fallido. Credenciales incorrectas para el usuario {LoginUsername}", username);

                    ViewBag.Error = "Credenciales inválidas";

                    stopwatch.Stop();
                    _logger.LogInformation("Fin AuthController.Login (POST). Rechazado. Tiempo de ejecución: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                    return View();
                }

                _logger.LogInformation("Evento de autenticación exitoso para el usuario {LoginUsername}", username);

                HttpContext.Session.SetString("User", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);

                stopwatch.Stop();
                _logger.LogInformation("Fin AuthController.Login (POST). Redireccionando a Dashboard. Tiempo de ejecución: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error crítico en AuthController.Login (POST) durante autenticación de {LoginUsername}. Tiempo: {ElapsedMilliseconds} ms", username, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public IActionResult Dashboard()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var sessionUser = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio AuthController.Dashboard");
            _logger.LogInformation("Usuario: {User} IP: {IP} intentando acceder al Dashboard", sessionUser, ip);

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    _logger.LogWarning("El usuario {User} intentó entrar al Dashboard sin una sesión activa. Redirigiendo al Login.", sessionUser);

                    stopwatch.Stop();
                    _logger.LogInformation("Fin AuthController.Dashboard. Redireccionado a Login. Tiempo: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                    return RedirectToAction("Login");
                }

                var user = _db.Users.Find(userId.Value);

                stopwatch.Stop();
                _logger.LogInformation("Fin AuthController.Dashboard. Acceso permitido. Tiempo de ejecución: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                return View(user);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error crítico en AuthController.Dashboard. Tiempo: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public IActionResult Logout()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var sessionUser = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio AuthController.Logout");
            _logger.LogInformation("Usuario: {User} IP: {IP} solicitando cierre de sesión", sessionUser, ip);

            try
            {
                HttpContext.Session.Clear();

                _logger.LogInformation("Evento de autenticación: Sesión finalizada para {User}", sessionUser);

                stopwatch.Stop();
                _logger.LogInformation("Fin AuthController.Logout. Tiempo de ejecución: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error crítico en AuthController.Logout. Tiempo: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}