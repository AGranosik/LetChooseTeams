using Asp.Versioning;
using LCT.Api.Configuration;
using LCT.Application;
using LCT.Infrastructure;
using LCT.Infrastructure.ClientCommunication.Hubs;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

namespace LCT.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHealthChecks();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
            });

            services.AddInfrastructure()
                .AddApplication();

            AddApiVersioning(services);

            services.AddSignalR();
            Console.WriteLine("Configuration finished.");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UserErrorLogging();
            app.UseCors(builder => builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials());

            ConfigureLogger();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TournamentHub>("/hubs/actions");
                endpoints.MapHealthChecks("/liveness");
            });

        }

        private static void AddApiVersioning(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddProblemDetails()
                .AddApiVersioning(o => {
                    o.ApiVersionReader = new HeaderApiVersionReader("api-version");
                    o.DefaultApiVersion = new ApiVersion(1.0);
                    o.AssumeDefaultVersionWhenUnspecified = true;
                }).AddMvc();
        }

        private void ConfigureLogger()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElasticSink(environment))
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();
        }
        private ElasticsearchSinkOptions ConfigureElasticSink(string environment)
        {
            return new ElasticsearchSinkOptions(new Uri (_configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}
