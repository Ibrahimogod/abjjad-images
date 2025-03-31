using System.Diagnostics;
using Abjjad.Images.API.Constants;

namespace Abjjad.Images.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Request.Headers[Headers.CorrelationId].ToString();

        try
        {
            await _next(context);
            
            stopwatch.Stop();
            _logger.LogInformation(
                "Request {RequestId} {Method} {Path} completed with status code {StatusCode} in {ElapsedMilliseconds}ms [CorrelationId: {CorrelationId}]",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                correlationId);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Request {RequestId} {Method} {Path} failed with exception after {ElapsedMilliseconds}ms [CorrelationId: {CorrelationId}]",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                correlationId);
            throw;
        }
    }
} 