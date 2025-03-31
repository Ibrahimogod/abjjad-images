using System.Diagnostics;
using Abjjad.Images.API.Constants;

namespace Abjjad.Images.API.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[Headers.CorrelationId].ToString();
        
        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers[Headers.CorrelationId] = correlationId;
        }

        context.Response.Headers[Headers.CorrelationId] = correlationId;
        await _next(context);
    }
} 