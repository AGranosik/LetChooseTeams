using LCT.Api.Configuration;
using LCT.Application;
using LCT.Infrastructure;
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
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddInfrastructure()
                .AddApplication();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UserErrorLogging();

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElasticSink(environment))
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private ElasticsearchSinkOptions ConfigureElasticSink(string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(_configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}
