using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Panda.Data;

namespace Panda.IntegrationTests
{
    public class PandaApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove real DB
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PandaDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory DB
                services.AddDbContext<PandaDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        }
    }
}
