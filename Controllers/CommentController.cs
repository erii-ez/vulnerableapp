using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace VulnerableApp.Controllers
{
    public class CommentController : Controller
    {
        private static List<string> _comments = new();
        private readonly ILogger<CommentController> _logger;

        public CommentController(ILogger<CommentController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var usuario = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio CommentController.Index | Usuario: {Usuario} | IP: {IP}", usuario, ip);

            try
            {
                return View(_comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CommentController.Index");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin CommentController.Index | Tiempo de ejecución: {TiempoMs} ms", stopwatch.ElapsedMilliseconds);
            }
        }

        [HttpPost]
        public IActionResult AddComment(string comment)
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var usuario = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio CommentController.AddComment | Usuario: {Usuario} | IP: {IP} | Comentario: {Comment}", usuario, ip, comment);

            try
            {
                if (!string.IsNullOrEmpty(comment))
                {
                    _comments.Add(comment);
                }
                else
                {
                    _logger.LogWarning("El usuario {Usuario} intentó enviar un comentario vacío.", usuario);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar comentario por el usuario {Usuario}", usuario);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin CommentController.AddComment | Tiempo de ejecución: {TiempoMs} ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
} 