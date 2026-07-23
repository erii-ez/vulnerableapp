using System.Diagnostics;
using Serilog.Context; 

namespace VulnerableApp.Middlewares
{
    public class GlobalLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalLoggingMiddleware> _logger;

        public GlobalLoggingMiddleware(RequestDelegate next, ILogger<GlobalLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var correlationId = Guid.NewGuid().ToString();

            // 1. Agregar a los headers de respuesta para el cliente
            context.Response.Headers.Append("X-Correlation-ID", correlationId);
            
            // --- INICIO DE REMEDIACIONES ZAP ---
            // Remediación para CSP (Hallazgo 1)
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; form-action 'self'; base-uri 'self'; frame-ancestors 'none';");
            // Remediación para Anti-clickjacking (Hallazgo 2)
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            // --- FIN DE REMEDIACIONES ZAP ---

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _next(context);

                    stopwatch.Stop();
                    _logger.LogInformation(
                        "HTTP {RequestMethod} {RequestPath} respondió con {StatusCode} en {ElapsedMilliseconds} ms",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();

                    _logger.LogError(ex, "Unhandled Exception: Error crítico interceptado por el middleware. Ruta: {RequestPath}. Tiempo: {ElapsedMilliseconds} ms", context.Request.Path, stopwatch.ElapsedMilliseconds);

                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Ocurrió un error interno en el servidor. Por favor, intente más tarde.");
                }
            }
        }
    }
}