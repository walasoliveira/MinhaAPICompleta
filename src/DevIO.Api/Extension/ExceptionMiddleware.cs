using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DevIO.Api.Extension
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                HandleExcpetion(context, ex);
            }
        }

        private static void HandleExcpetion(HttpContext context, Exception ex)
        {
            ex.Ship(context);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}
