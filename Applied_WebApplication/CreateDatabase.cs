using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Applied_WebApplication
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CreateDatabase
    {
        private readonly RequestDelegate _next;
        private readonly ConnectionClass _Class = new();

        public CreateDatabase(RequestDelegate next)
        {
            if(_Class.DBFile_Exist)
            {
                _next = next;
            }
            else
            {
                _next = next;
            }
            

        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CreateDatabaseExtensions
    {
        public static IApplicationBuilder UseCreateDatabase(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CreateDatabase>();
        }
    }
}
