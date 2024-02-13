using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Web.Settings
{
    public class Dependencies
    {
        public static void Inject(WebApplicationBuilder app)
        {
            app.Services.AddControllers();
            app.Services.AddEndpointsApiExplorer();
            app.Services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            app.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            app.Services.AddCors(options =>
            {
                options.AddPolicy("Access-Control-Allow-Origin",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });
            AddRepositories(app.Services);
            AddServices(app.Services);
        }

        private static void AddRepositories(IServiceCollection services)
        {
            
        }

        private static void AddServices(IServiceCollection services)
        {
            
        }
    }
}
