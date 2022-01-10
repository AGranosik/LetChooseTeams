using LCT.Infrastructure;
using LCT.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

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
            services.AddInfrastructure();
        }
    }
}
