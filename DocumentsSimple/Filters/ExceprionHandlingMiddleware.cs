using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentsSimple.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DocumentsSimple.Filters
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceprionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceprionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (HttpStatusCodeException ex)
            {
                if (httpContext.Response.HasStarted)
                {
                    //log error
                    throw;
                }

                httpContext.Response.Clear();
                httpContext.Response.StatusCode = ex.StatusCode;
                httpContext.Response.ContentType = ex.ContentType;

                await httpContext.Response.WriteAsync(ex.Message);

                return;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceprionHandlingMiddleware>();
        }
    }
}
