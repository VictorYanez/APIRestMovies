using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeliculasAPIC;
using PeliculasAPIC.Helpers;
using PeliculasAPIC.Servicios;

namespace PeliculasAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        #region Configuración de Servicios
        // Se ejecuta al iniciar la aplicación. Aquí se registran los servicios necesarios.
        public void ConfigureServices(IServiceCollection services)
        {
            // 🔄 AutoMapper: registra todos los perfiles encontrados en los ensamblados cargados
            services.AddAutoMapper(typeof(Startup));

            // 🔄 Servicio para Almecenar Archivos Localmente
            services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
            services.AddHttpContextAccessor();

            // Configuración de EF Core con SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions
                    .EnableRetryOnFailure()
                    .UseNetTopologySuite() // 👈 Para datos geoespaciales 
                ));


            // ✅ AGREGAR LOGGING AQUÍ (en Startup)
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            });


            // Registro de controladores (API)
            // Se incluye soporte para JSON automáticamente y Patch
            services.AddControllers()
                .AddNewtonsoftJson();

            // Registro de AutoMapper (usa perfiles definidos en el proyecto)
            //services.AddAutoMapper(typeof(Startup));

            // Acceso al contexto HTTP (útil para obtener usuario, headers, etc.)
            services.AddHttpContextAccessor();

            // Aquí puedes agregar otros servicios como JWT, Swagger, CORS, etc.
        }
        #endregion

        #region Configuración del Pipeline HTTP
        // Se ejecuta en tiempo de ejecución. Define cómo se manejan las solicitudes HTTP.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ✅ Obtener logger desde los servicios
            var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
            logger.LogInformation("Iniciando configuración de la aplicación");

            // Página de errores detallados en desarrollo
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Redirección a HTTPS
            app.UseHttpsRedirection();

            // Archivos estáticos (si usas wwwroot)
            app.UseStaticFiles();

            // Enrutamiento
            app.UseRouting();

            // Autenticación y autorización (si se configura)
            app.UseAuthorization();

            // Mapeo de endpoints (controladores)
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            logger.LogInformation("Configuración de aplicación completada");
        }
        #endregion
    }
}
