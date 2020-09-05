using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.Api.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection WebApiConfig(this IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

            services.AddCors(o =>
            {
                o.AddPolicy("Developtment", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });

                o.AddPolicy("Production", builder =>
                {
                    builder.WithMethods("GET")
                           .WithOrigins("http://desenvolvedor.io")
                           .SetIsOriginAllowedToAllowWildcardSubdomains()
                           //.WithHeaders(HeaderNames.ContentType, "x-custom-header")
                           .AllowAnyHeader();
                });
            });

            return services;
        }

        public static IApplicationBuilder UserMvcConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            return app;
        }
    }
}
