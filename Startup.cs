using Microsoft.OpenApi.Models;
using DatabaseApi.Controllers;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DatabaseApi
{    

    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Method gets called by Runtime -> Used to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            // Register Swagger generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bursary Database API", Version = "v1" });

                // Generate xml docs to drive swagger docs
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
                c.CustomOperationIds(apiDescription =>
                {
                    return apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                });
            }).AddSwaggerGenNewtonsoftSupport();
            
            // Registering Different Controllers with Dependency Injection (DI)
            services.AddScoped<DepartmentsController>();
            services.AddScoped<UniversitiesController>();
            services.AddScoped<StudentsController>();
            services.AddScoped<StudentsAllocationController>();
            services.AddScoped<UniversityApplicationController>();
            services.AddScoped<BursaryAllocationController>();
            services.AddScoped<UserController>();
            services.AddScoped<UserContactController>();
            services.AddScoped<ErrorController>();
            
           
        }

        // Configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bursary Database API V1");
                c.DisplayOperationId();
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}